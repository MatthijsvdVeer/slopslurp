namespace SlopSlurp.Rules.Models;

public class ValidationResult
{
    public List<RuleViolation> Violations { get; init; } = [];

    public int TotalViolations => Violations.Count;

    public bool HasViolations => Violations.Count > 0;

    public IEnumerable<IGrouping<string, RuleViolation>> ViolationsByRule =>
        Violations.GroupBy(v => v.RuleId);

    public string Summary =>
        HasViolations
            ? $"Found {TotalViolations} violation(s) across {ViolationsByRule.Count()} rule(s)."
            : "No violations found. Your text looks clean!";
}
