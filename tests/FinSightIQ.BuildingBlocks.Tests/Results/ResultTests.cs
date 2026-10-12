using FinSightIQ.BuildingBlocks.Results;

namespace FinSightIQ.BuildingBlocks.Tests.Results;

public class ResultTests
{
    private static readonly Error SampleError = new("SYMBOL_NOT_FOUND", "Ticker is not supported.");

    [Fact]
    public void Success_WithValue_ExposesValueAndNoError()
    {
        Result<int> result = Result<int>.Success(42);

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(42, result.Value);
        Assert.Throws<InvalidOperationException>(() => result.Error);
    }

    [Fact]
    public void Success_WithNullReference_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => Result<string>.Success(null!));
    }

    [Fact]
    public void Failure_WithError_ExposesErrorAndNoValue()
    {
        Result<string> result = Result<string>.Failure(SampleError);

        Assert.True(result.IsFailure);
        Assert.False(result.IsSuccess);
        Assert.Same(SampleError, result.Error);
        Assert.Throws<InvalidOperationException>(() => result.Value);
    }

    [Fact]
    public void Failure_WithCodeAndMessage_CreatesError()
    {
        Result<string> result = Result<string>.Failure("VALIDATION_FAILED", "Bad input.");

        Assert.Equal("VALIDATION_FAILED", result.Error.Code);
        Assert.Equal("Bad input.", result.Error.Message);
    }

    [Fact]
    public void Failure_WithNullError_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => Result<string>.Failure((Error)null!));
        Assert.Throws<ArgumentNullException>(() => Result.Failure((Error)null!));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("lower_case")]
    [InlineData("HAS SPACE")]
    [InlineData("_LEADING")]
    [InlineData("TRAILING_")]
    [InlineData("DOUBLE__UNDERSCORE")]
    public void Failure_WithInvalidCode_Throws(string? code)
    {
        Assert.Throws<ArgumentException>(() => Result<int>.Failure(code!, "message"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Failure_WithBlankMessage_Throws(string? message)
    {
        Assert.ThrowsAny<ArgumentException>(() => Result<int>.Failure("SOME_CODE", message!));
    }

    [Fact]
    public void TryGetValue_OnSuccess_ReturnsTrueAndValue()
    {
        bool found = Result<string>.Success("ok").TryGetValue(out string value);

        Assert.True(found);
        Assert.Equal("ok", value);
    }

    [Fact]
    public void TryGetValue_OnFailure_ReturnsFalse()
    {
        bool found = Result<string>.Failure(SampleError).TryGetValue(out string value);

        Assert.False(found);
        Assert.Null(value);
    }

    [Fact]
    public void NonGenericSuccess_HasNoError()
    {
        Result result = Result.Success();

        Assert.True(result.IsSuccess);
        Assert.Throws<InvalidOperationException>(() => result.Error);
    }

    [Fact]
    public void NonGenericFailure_HasError()
    {
        Result result = Result.Failure(SampleError);

        Assert.True(result.IsFailure);
        Assert.Same(SampleError, result.Error);
    }

    [Fact]
    public void Error_Equality_IsValueBased()
    {
        Assert.Equal(new Error("A_B", "m"), new Error("A_B", "m"));
    }
}
