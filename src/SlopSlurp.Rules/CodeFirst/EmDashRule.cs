using System.Text.RegularExpressions;
using SlopSlurp.Rules.Models;

namespace SlopSlurp.Rules.CodeFirst;

public partial class EmDashRule : IValidationRule
{
    public RuleDefinition Definition => RuleRegistry.GetRule("SL004")!;

    [GeneratedRegex(@"\u2014|\u2013|(?<!\-)--(?!\-)", RegexOptions.Compiled)]
    private static partial Regex EmDashPattern();

    public Task<IEnumerable<RuleViolation>> ValidateAsync(string text, CancellationToken cancellationToken = default)
    {
        var violations = new List<RuleViolation>();
        var matches = EmDashPattern().Matches(text);

        foreach (Match match in matches)
        {
            violations.Add(new RuleViolation(
                RuleId: Definition.Id,
                MatchedText: match.Value,
                StartIndex: match.Index,
                Length: match.Length,
                Explanation: match.Value switch
                {
                    "\u2014" => "Em dash (—) detected. Overuse of em dashes is a common AI writing tell.",
                    "\u2013" => "En dash (–) detected. Often used by AI as a dramatic pause or pivot.",
                    _ => "Double hyphen (--) detected. Often used as an em dash substitute by AI."
                }));
        }

        return Task.FromResult<IEnumerable<RuleViolation>>(violations);
    }
}
