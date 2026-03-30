namespace SlopSlurp.Rules.Models;

public record RuleViolation(
    string RuleId,
    string MatchedText,
    int StartIndex,
    int Length,
    string Explanation);
