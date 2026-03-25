using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using SlopSlurp.Rules.CodeFirst;
using SlopSlurp.Rules.LlmPowered;
using SlopSlurp.Rules.Models;

namespace SlopSlurp.Rules;

public class ValidationEngine
{
    private readonly List<IValidationRule> _codeFirstRules;
    private readonly List<LlmValidationRule> _llmRules;
    private readonly ILogger<ValidationEngine> _logger;

    public int TotalRuleCount => _codeFirstRules.Count + _llmRules.Count;

    public ValidationEngine(
        IChatClient chatClient,
        ILogger<ValidationEngine> logger)
    {
        _logger = logger;
        _codeFirstRules =
        [
            new EmDashRule(),
            new EmojiRule()
        ];
        _llmRules = RuleRegistry.LlmRules
            .Select(def => new LlmValidationRule(chatClient, def))
            .ToList();
    }

    public async Task<ValidationResult> ValidateAsync(
        string text,
        IProgress<ValidationProgressUpdate>? progress = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new ValidationResult();

        if (text.Length > 1000)
            text = text[..1000];

        var allViolations = new List<RuleViolation>();
        var completed = 0;
        var total = TotalRuleCount;

        // Phase 1: Run code-first rules (fast, deterministic)
        foreach (var rule in _codeFirstRules)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var violations = (await rule.ValidateAsync(text, cancellationToken)).ToList();
                allViolations.AddRange(violations);
                completed++;
                progress?.Report(new ValidationProgressUpdate(
                    completed, total, rule.Definition.Name, violations));
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running code-first rule {RuleId}", rule.Definition.Id);
                completed++;
                progress?.Report(new ValidationProgressUpdate(
                    completed, total, rule.Definition.Name, []));
            }
        }

        // Phase 2: Fire all LLM rules in parallel, report as each completes
        var llmTasks = _llmRules.Select(async rule =>
        {
            try
            {
                var violations = (await rule.ValidateAsync(text, cancellationToken)).ToList();
                return (rule.Definition, Violations: violations);
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running LLM rule {RuleId}", rule.Definition.Id);
                return (rule.Definition, Violations: new List<RuleViolation>());
            }
        }).ToList();

        var remaining = new List<Task<(RuleDefinition Definition, List<RuleViolation> Violations)>>(llmTasks);

        while (remaining.Count > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var completedTask = await Task.WhenAny(remaining);
            remaining.Remove(completedTask);

            var result = await completedTask;
            allViolations.AddRange(result.Violations);
            completed++;

            progress?.Report(new ValidationProgressUpdate(
                completed, total, result.Definition.Name, result.Violations));
        }

        allViolations.Sort((a, b) => a.StartIndex.CompareTo(b.StartIndex));

        return new ValidationResult { Violations = allViolations };
    }
}
