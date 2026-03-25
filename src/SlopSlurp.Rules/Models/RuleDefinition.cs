namespace SlopSlurp.Rules.Models;

public record RuleDefinition(
    string Id,
    string Name,
    string Category,
    string Description,
    string Example);
