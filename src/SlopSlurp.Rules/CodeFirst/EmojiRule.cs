using System.Text.RegularExpressions;
using SlopSlurp.Rules.Models;

namespace SlopSlurp.Rules.CodeFirst;

public partial class EmojiRule : IValidationRule
{
    public RuleDefinition Definition => RuleRegistry.GetRule("SL012")!;

    // Matches emoji, unicode arrows (→ ← ↑ ↓ ⇒ ⇐ etc.), smart/curly quotes, and other decorative unicode
    [GeneratedRegex(
        @"[\u2190-\u21FF]" +           // Arrows
        @"|[\u2700-\u27BF]" +          // Dingbats
        @"|[\u2600-\u26FF]" +          // Miscellaneous Symbols
        @"|[\u2B50-\u2B55]" +          // Stars/circles
        @"|[\u231A-\u231B]" +          // Watch/hourglass
        @"|[\u23E9-\u23F3]" +          // Media controls
        @"|[\u23F8-\u23FA]" +          // More media controls
        @"|[\u25AA-\u25FE]" +          // Geometric shapes
        @"|[\u2018-\u201F]" +          // Smart/curly quotes
        @"|[\uD83C-\uDBFF][\uDC00-\uDFFF]" + // Surrogate pairs (emoji)
        @"|[\u200D]" +                 // Zero-width joiner (emoji sequences)
        @"|[\uFE0F]",                  // Variation selector (emoji presentation)
        RegexOptions.Compiled)]
    private static partial Regex UnicodeDecorationPattern();

    public Task<IEnumerable<RuleViolation>> ValidateAsync(string text, CancellationToken cancellationToken = default)
    {
        var violations = new List<RuleViolation>();
        var matches = UnicodeDecorationPattern().Matches(text);

        foreach (Match match in matches)
        {
            string explanation = char.GetUnicodeCategory(match.Value[0]) switch
            {
                System.Globalization.UnicodeCategory.InitialQuotePunctuation or
                System.Globalization.UnicodeCategory.FinalQuotePunctuation =>
                    "Smart/curly quote detected. Real writers typing in a text editor produce straight quotes.",
                _ when match.Value[0] >= '\u2190' && match.Value[0] <= '\u21FF' =>
                    "Unicode arrow detected. Real writers use -> or => instead of fancy arrows.",
                _ => "Unicode decoration detected. Special characters that can't be easily typed on a standard keyboard are a sign of AI generation."
            };

            violations.Add(new RuleViolation(
                RuleId: Definition.Id,
                MatchedText: match.Value,
                StartIndex: match.Index,
                Length: match.Length,
                Explanation: explanation));
        }

        return Task.FromResult<IEnumerable<RuleViolation>>(violations);
    }
}
