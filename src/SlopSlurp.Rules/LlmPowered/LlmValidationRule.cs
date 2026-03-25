using SlopSlurp.Rules.Models;

namespace SlopSlurp.Rules.LlmPowered;

public class LlmValidationRule : IValidationRule
{
    private readonly TropeAnalysisAgent _agent;
    private readonly RuleDefinition _definition;

    public LlmValidationRule(TropeAnalysisAgent agent, RuleDefinition definition)
    {
        _agent = agent;
        _definition = definition;
    }

    public RuleDefinition Definition => _definition;

    public async Task<IEnumerable<RuleViolation>> ValidateAsync(string text)
    {
        // The TropeAnalysisAgent handles all LLM rules in a single call,
        // so individual LlmValidationRule instances just filter results.
        // The ValidationEngine calls the agent once and distributes results.
        return await Task.FromResult<IEnumerable<RuleViolation>>([]);
    }
}
