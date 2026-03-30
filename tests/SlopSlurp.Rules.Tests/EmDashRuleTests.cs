using SlopSlurp.Rules.CodeFirst;
using SlopSlurp.Rules.Models;

namespace SlopSlurp.Rules.Tests;

public class EmDashRuleTests
{
    private readonly EmDashRule _rule = new();

    [Fact]
    public async Task DetectsEmDash()
    {
        var text = "The problem\u2014and this is important\u2014is systemic.";
        var violations = (await _rule.ValidateAsync(text)).ToList();

        Assert.Equal(2, violations.Count);
        Assert.All(violations, v => Assert.Equal("SL004", v.RuleId));
        Assert.All(violations, v => Assert.Equal("\u2014", v.MatchedText));
    }

    [Fact]
    public async Task DetectsEnDash()
    {
        var text = "The years 2020\u20132025 were transformative.";
        var violations = (await _rule.ValidateAsync(text)).ToList();

        Assert.Single(violations);
        Assert.Equal("\u2013", violations[0].MatchedText);
        Assert.Equal(14, violations[0].StartIndex);
    }

    [Fact]
    public async Task DetectsDoubleHyphen()
    {
        var text = "This is important -- very important -- to understand.";
        var violations = (await _rule.ValidateAsync(text)).ToList();

        Assert.Equal(2, violations.Count);
        Assert.All(violations, v => Assert.Equal("--", v.MatchedText));
    }

    [Fact]
    public async Task DoesNotFlagSingleHyphen()
    {
        var text = "This is a well-known fact about co-operation.";
        var violations = (await _rule.ValidateAsync(text)).ToList();

        Assert.Empty(violations);
    }

    [Fact]
    public async Task DoesNotFlagTripleHyphen()
    {
        var text = "Use --- for horizontal rules in markdown.";
        var violations = (await _rule.ValidateAsync(text)).ToList();

        Assert.Empty(violations);
    }

    [Fact]
    public async Task ReturnsCorrectStartIndex()
    {
        var text = "Hello\u2014world";
        var violations = (await _rule.ValidateAsync(text)).ToList();

        Assert.Single(violations);
        Assert.Equal(5, violations[0].StartIndex);
        Assert.Equal(1, violations[0].Length);
    }

    [Fact]
    public async Task HandlesEmptyString()
    {
        var violations = (await _rule.ValidateAsync("")).ToList();
        Assert.Empty(violations);
    }

    [Fact]
    public async Task DetectsMixedDashTypes()
    {
        var text = "First\u2014second\u2013third--fourth";
        var violations = (await _rule.ValidateAsync(text)).ToList();

        Assert.Equal(3, violations.Count);
    }
}
