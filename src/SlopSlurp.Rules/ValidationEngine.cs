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

    public async Task<ValidationResult> ValidateAsync(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new ValidationResult();

        if (text.Length > 1000)
            text = text[..1000];

        var allViolations = new List<RuleViolation>();

        // Phase 1: Run code-first rules (fast, deterministic)
        foreach (var rule in _codeFirstRules)
        {
            try
            {
                var violations = await rule.ValidateAsync(text);
                allViolations.AddRange(violations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running code-first rule {RuleId}", rule.Definition.Id);
            }
        }

        // Phase 2: Run all LLM rules in parallel
        var llmTasks = _llmRules.Select(async rule =>
        {
            try
            {
                return await rule.ValidateAsync(text);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running LLM rule {RuleId}", rule.Definition.Id);
                return Enumerable.Empty<RuleViolation>();
            }
        });

        var llmResults = await Task.WhenAll(llmTasks);
        foreach (var violations in llmResults)
        {
            allViolations.AddRange(violations);
        }

        // Sort by position in text
        allViolations.Sort((a, b) => a.StartIndex.CompareTo(b.StartIndex));

        return new ValidationResult { Violations = allViolations };
    }
}
