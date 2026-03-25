using SlopSlurp.Rules.Models;

namespace SlopSlurp.Rules;

public record ValidationProgressUpdate(
    int CompletedCount,
    int TotalCount,
    string CurrentRuleName,
    IReadOnlyList<RuleViolation> NewViolations)
{
    public double Progress => TotalCount > 0 ? (double)CompletedCount / TotalCount : 0;
}
