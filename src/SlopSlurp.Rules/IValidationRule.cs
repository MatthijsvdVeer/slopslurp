using SlopSlurp.Rules.Models;

namespace SlopSlurp.Rules;

public interface IValidationRule
{
    RuleDefinition Definition { get; }
    Task<IEnumerable<RuleViolation>> ValidateAsync(string text, CancellationToken cancellationToken = default);
}
