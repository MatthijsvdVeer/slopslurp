using SlopSlurp.Rules.CodeFirst;
using SlopSlurp.Rules.Models;

namespace SlopSlurp.Rules.Tests;

public class EmojiRuleTests
{
    private readonly EmojiRule _rule = new();

    [Fact]
    public async Task DetectsUnicodeArrows()
    {
        var text = "Input \u2192 Processing \u2192 Output";
        var violations = (await _rule.ValidateAsync(text)).ToList();

        Assert.Equal(2, violations.Count);
        Assert.All(violations, v => Assert.Equal("SL012", v.RuleId));
    }

    [Fact]
    public async Task DetectsSmartQuotes()
    {
        var text = "He said \u201CHello\u201D to the crowd.";
        var violations = (await _rule.ValidateAsync(text)).ToList();

        Assert.Equal(2, violations.Count);
        Assert.All(violations, v =>
            Assert.Contains("Smart/curly quote", v.Explanation));
    }

    [Fact]
    public async Task DoesNotFlagPlainText()
    {
        var text = "This is plain text with no special characters. It uses -> arrows and \"straight quotes\".";
        var violations = (await _rule.ValidateAsync(text)).ToList();

        Assert.Empty(violations);
    }

    [Fact]
    public async Task DetectsDingbats()
    {
        var text = "Check this out \u2714 it works!";
        var violations = (await _rule.ValidateAsync(text)).ToList();

        Assert.NotEmpty(violations);
    }

    [Fact]
    public async Task ReturnsCorrectStartIndex()
    {
        var text = "Go \u2192 here";
        var violations = (await _rule.ValidateAsync(text)).ToList();

        Assert.Single(violations);
        Assert.Equal(3, violations[0].StartIndex);
    }

    [Fact]
    public async Task HandlesEmptyString()
    {
        var violations = (await _rule.ValidateAsync("")).ToList();
        Assert.Empty(violations);
    }

    [Fact]
    public async Task DetectsMultipleTypes()
    {
        var text = "\u201CHello\u201D \u2192 World \u2714";
        var violations = (await _rule.ValidateAsync(text)).ToList();

        Assert.True(violations.Count >= 3);
    }
}
