using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using SlopSlurp.Rules.Models;

namespace SlopSlurp.Rules.LlmPowered;

public class TropeAnalysisAgent
{
    private readonly IChatClient _chatClient;
    private readonly ILogger<TropeAnalysisAgent> _logger;
    private readonly string _systemPrompt;

    public TropeAnalysisAgent(IChatClient chatClient, ILogger<TropeAnalysisAgent> logger)
    {
        _chatClient = chatClient;
        _logger = logger;
        _systemPrompt = BuildSystemPrompt();
    }

    public async Task<List<LlmViolationResult>> AnalyzeAsync(string text)
    {
        try
        {
            var messages = new List<ChatMessage>
            {
                new(ChatRole.System, _systemPrompt),
                new(ChatRole.User, text)
            };

            var options = new ChatOptions
            {
                Temperature = 0.1f,
                ResponseFormat = ChatResponseFormat.Json
            };

            var response = await _chatClient.GetResponseAsync(messages, options);
            var responseText = response.Text;

            var json = ExtractJson(responseText);
            var result = JsonSerializer.Deserialize<LlmAnalysisResponse>(json, JsonOptions);
            return result?.Violations ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during LLM trope analysis");
            return [];
        }
    }

    private static string ExtractJson(string text)
    {
        var jsonStart = text.IndexOf("```json", StringComparison.OrdinalIgnoreCase);
        if (jsonStart >= 0)
        {
            jsonStart = text.IndexOf('\n', jsonStart) + 1;
            var jsonEnd = text.IndexOf("```", jsonStart, StringComparison.Ordinal);
            if (jsonEnd > jsonStart)
                return text[jsonStart..jsonEnd].Trim();
        }

        jsonStart = text.IndexOf("```", StringComparison.Ordinal);
        if (jsonStart >= 0)
        {
            jsonStart = text.IndexOf('\n', jsonStart) + 1;
            var jsonEnd = text.IndexOf("```", jsonStart, StringComparison.Ordinal);
            if (jsonEnd > jsonStart)
                return text[jsonStart..jsonEnd].Trim();
        }

        return text.Trim();
    }

    private static string BuildSystemPrompt()
    {
        var tropes = RuleRegistry.LlmRules;
        var tropeDescriptions = string.Join("\n\n", tropes.Select(t =>
            $"### {t.Id}: {t.Name}\n" +
            $"Category: {t.Category}\n" +
            $"Description: {t.Description}\n" +
            $"Example: {t.Example}"));

        return $$"""
            You are a precise AI writing trope detector. Your job is to analyze text and identify instances of known AI writing tropes.

            ## Trope Definitions

            {{tropeDescriptions}}

            ## Instructions

            1. Analyze the provided text against ALL of the trope definitions above.
            2. For each violation found, identify the EXACT text that matches and explain why it matches.
            3. Be precise with startIndex — it must be the exact character position (0-based) of the matched text in the input.
            4. Only flag clear violations. Do not flag borderline cases.
            5. A piece of text can violate multiple rules.

            ## Output Format

            Respond with ONLY valid JSON in this exact format:
            {
              "violations": [
                {
                  "ruleId": "SL001",
                  "matchedText": "the exact text that violates the rule",
                  "startIndex": 0,
                  "explanation": "Why this text is a violation of the rule"
                }
              ]
            }

            If no violations are found, respond with: { "violations": [] }
            """;
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
}

public class LlmAnalysisResponse
{
    [JsonPropertyName("violations")]
    public List<LlmViolationResult> Violations { get; set; } = [];
}

public class LlmViolationResult
{
    [JsonPropertyName("ruleId")]
    public string RuleId { get; set; } = string.Empty;

    [JsonPropertyName("matchedText")]
    public string MatchedText { get; set; } = string.Empty;

    [JsonPropertyName("startIndex")]
    public int StartIndex { get; set; }

    [JsonPropertyName("explanation")]
    public string Explanation { get; set; } = string.Empty;
}
