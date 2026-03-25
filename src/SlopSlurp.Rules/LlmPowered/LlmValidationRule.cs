using System.Text.Json;
using Microsoft.Extensions.AI;
using SlopSlurp.Rules.Models;

namespace SlopSlurp.Rules.LlmPowered;

public class LlmValidationRule : IValidationRule
{
    private readonly IChatClient _chatClient;
    private readonly RuleDefinition _definition;

    public LlmValidationRule(IChatClient chatClient, RuleDefinition definition)
    {
        _chatClient = chatClient;
        _definition = definition;
    }

    public RuleDefinition Definition => _definition;

    public async Task<IEnumerable<RuleViolation>> ValidateAsync(string text, CancellationToken cancellationToken = default)
    {
        try
        {
            var prompt = $"""
                {BuildRulePrompt()}

                ---
                TEXT TO ANALYZE:
                {text}
                ---
                """;

            var options = new ChatOptions
            {
                Temperature = 0.1f,
                ResponseFormat = ChatResponseFormat.ForJsonSchema<LlmRuleResponse>(
                    schemaName: "trope_detection")
            };

            var response = await _chatClient.GetResponseAsync(prompt, options, cancellationToken);
            var responseText = response.Text.Trim();

            // Strip markdown fences if present
            if (responseText.StartsWith("```"))
            {
                var lines = responseText.Split('\n');
                responseText = string.Join('\n',
                    lines.Skip(1).TakeWhile(l => !l.TrimStart().StartsWith("```")));
            }

            var result = JsonSerializer.Deserialize<LlmRuleResponse>(responseText, JsonOptions);

            if (result is not { Detected: true })
                return [];

            var matchedText = result.OffendingText ?? "";
            var startIndex = text.IndexOf(matchedText, StringComparison.OrdinalIgnoreCase);
            if (startIndex < 0) startIndex = 0;

            return
            [
                new RuleViolation(
                    RuleId: _definition.Id,
                    MatchedText: matchedText,
                    StartIndex: startIndex,
                    Length: matchedText.Length,
                    Explanation: result.Explanation ?? $"Detected {_definition.Name} trope.")
            ];
        }
        catch
        {
            return [];
        }
    }

    private string BuildRulePrompt() =>
        $"""
        You are an AI writing pattern detector. Analyze the following text for the "{_definition.Name}" trope.

        {_definition.Description}

        Example: {_definition.Example}

        Return whether this pattern is detected and quote the offending text.
        """;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
}
