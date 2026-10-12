using FinSightIQ.BuildingBlocks.Middleware;

namespace FinSightIQ.BuildingBlocks.Tests.Middleware;

public class CorrelationIdTests
{
    [Fact]
    public void HeaderName_MatchesDocumentedConvention()
    {
        Assert.Equal("X-Correlation-Id", CorrelationId.HeaderName);
    }

    [Fact]
    public void NewId_GeneratesValidUniqueIdentifiers()
    {
        CorrelationId first = CorrelationId.NewId();
        CorrelationId second = CorrelationId.NewId();

        Assert.NotEqual(first, second);
        Assert.True(CorrelationId.TryParse(first.Value, out _));
    }

    [Theory]
    [InlineData("abc123")]
    [InlineData("4bf92f3577b34da6a3ce929d0e0e4736")]
    [InlineData("req_1.2-3")]
    public void Parse_WithValidValue_PreservesValue(string value)
    {
        CorrelationId id = CorrelationId.Parse(value);

        Assert.Equal(value, id.Value);
        Assert.Equal(value, id.ToString());
    }

    [Fact]
    public void Parse_WithMaxLengthValue_Succeeds()
    {
        Assert.True(CorrelationId.TryParse(new string('a', CorrelationId.MaxLength), out _));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("has space")]
    [InlineData("new\r\nline")]
    [InlineData("semi;colon")]
    [InlineData("ünïcode")]
    public void TryParse_WithInvalidValue_ReturnsFalse(string? value)
    {
        bool valid = CorrelationId.TryParse(value, out CorrelationId? id);

        Assert.False(valid);
        Assert.Null(id);
    }

    [Fact]
    public void TryParse_WithTooLongValue_ReturnsFalse()
    {
        Assert.False(CorrelationId.TryParse(new string('a', CorrelationId.MaxLength + 1), out _));
    }

    [Fact]
    public void Parse_WithInvalidValue_Throws()
    {
        Assert.Throws<ArgumentException>(() => CorrelationId.Parse("bad value"));
        Assert.Throws<ArgumentException>(() => CorrelationId.Parse(null!));
    }
}
