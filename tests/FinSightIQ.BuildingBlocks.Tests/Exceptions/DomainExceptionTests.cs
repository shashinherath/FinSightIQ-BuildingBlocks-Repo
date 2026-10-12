using FinSightIQ.BuildingBlocks.Exceptions;

namespace FinSightIQ.BuildingBlocks.Tests.Exceptions;

public class DomainExceptionTests
{
    [Fact]
    public void Constructor_SetsCodeAndMessage()
    {
        DomainException exception = new("INSUFFICIENT_QUANTITY", "Sell exceeds held quantity.");

        Assert.Equal("INSUFFICIENT_QUANTITY", exception.Code);
        Assert.Equal("Sell exceeds held quantity.", exception.Message);
        Assert.Null(exception.InnerException);
    }

    [Fact]
    public void Constructor_WithInnerException_PreservesIt()
    {
        InvalidOperationException inner = new("root cause");

        DomainException exception = new("SOME_CODE", "Wrapped.", inner);

        Assert.Same(inner, exception.InnerException);
    }

    [Fact]
    public void Constructor_WithNullInnerException_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new DomainException("SOME_CODE", "msg", null!));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not_upper")]
    public void Constructor_WithInvalidCode_Throws(string? code)
    {
        Assert.Throws<ArgumentException>(() => new DomainException(code!, "message"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithBlankMessage_Throws(string? message)
    {
        Assert.ThrowsAny<ArgumentException>(() => new DomainException("SOME_CODE", message!));
    }
}
