using Microsoft.Extensions.Logging;
using SlopSlurp.Rules.CodeFirst;
using SlopSlurp.Rules.LlmPowered;
using SlopSlurp.Rules.Models;

namespace SlopSlurp.Rules;

public class ValidationEngine
{
    private readonly List<IValidationRule> _codeFirstRules;
    private readonly TropeAnalysisAgent _tropeAgent;
    private readonly ILogger<ValidationEngine> _logger;

    public ValidationEngine(
        TropeAnalysisAgent tropeAgent,
        ILogger<ValidationEngine> logger)
    {
        _tropeAgent = tropeAgent;
        _logger = logger;
        _codeFirstRules =
        [
            new EmDashRule(),
            new EmojiRule()
        ];
    }

    public async Task<ValidationResult> ValidateAsync(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new ValidationResult();

        // Enforce 1000 char limit
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

        // Phase 2: Run LLM-powered analysis (single batched call)
        try
        {
            var llmResults = await _tropeAgent.AnalyzeAsync(text);

            foreach (var result in llmResults)
            {
                var ruleDef = RuleRegistry.GetRule(result.RuleId);
                if (ruleDef is null)
                {
                    _logger.LogWarning("LLM returned unknown rule ID: {RuleId}", result.RuleId);
                    continue;
                }

                // Skip code-first rules returned by LLM (we already handled them)
                if (ruleDef.Id is "SL004" or "SL012")
                    continue;

                // Validate and correct the startIndex if needed
                var startIndex = result.StartIndex;
                if (startIndex < 0 || startIndex >= text.Length)
                {
                    // Try to find the matched text in the input
                    startIndex = text.IndexOf(result.MatchedText, StringComparison.OrdinalIgnoreCase);
                    if (startIndex < 0) startIndex = 0;
                }

                allViolations.Add(new RuleViolation(
                    RuleId: result.RuleId,
                    MatchedText: result.MatchedText,
                    StartIndex: startIndex,
                    Length: result.MatchedText.Length,
                    Explanation: result.Explanation));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during LLM trope analysis");
        }

        // Sort by position in text
        allViolations.Sort((a, b) => a.StartIndex.CompareTo(b.StartIndex));

        return new ValidationResult { Violations = allViolations };
    }
}
