using System.Text.Json.Serialization;

namespace SlopSlurp.Rules.LlmPowered;

public class LlmRuleResponse
{
    [JsonPropertyName("detected")]
    public bool Detected { get; set; }

    [JsonPropertyName("offendingText")]
    public string? OffendingText { get; set; }

    [JsonPropertyName("explanation")]
    public string? Explanation { get; set; }
}
