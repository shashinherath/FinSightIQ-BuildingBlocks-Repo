namespace FinSightIQ.BuildingBlocks.Middleware;

/// <summary>
/// A validated request correlation identifier carried in the <c>X-Correlation-Id</c> header.
/// </summary>
/// <remarks>
/// This type is independent of HTTP and logging. Request middleware (gateway or web projects)
/// should call <see cref="TryParse"/> on incoming values and fall back to <see cref="NewId"/>
/// when the header is absent or invalid.
/// </remarks>
public sealed record CorrelationId
{
    /// <summary>The HTTP header name used to carry the identifier.</summary>
    public const string HeaderName = "X-Correlation-Id";

    /// <summary>The maximum accepted identifier length.</summary>
    public const int MaxLength = 64;

    private CorrelationId(string value)
    {
        Value = value;
    }

    /// <summary>Gets the identifier text.</summary>
    public string Value { get; }

    /// <summary>Generates a new random identifier (32 lowercase hexadecimal characters).</summary>
    public static CorrelationId NewId() => new(Guid.NewGuid().ToString("N"));

    /// <summary>Creates an identifier from a supplied value, validating it.</summary>
    /// <param name="value">1 to <see cref="MaxLength"/> characters from <c>A-Z a-z 0-9 - _ .</c>.</param>
    /// <exception cref="ArgumentException">The value is blank, too long or contains other characters.</exception>
    public static CorrelationId Parse(string value)
    {
        if (!TryParse(value, out CorrelationId? correlationId))
        {
            throw new ArgumentException(
                $"Correlation ID must be 1 to {MaxLength} characters from A-Z, a-z, 0-9, '-', '_' and '.'.",
                nameof(value));
        }

        return correlationId;
    }

    /// <summary>Attempts to create an identifier from untrusted input without throwing.</summary>
    /// <param name="value">The candidate value, for example a header value.</param>
    /// <param name="correlationId">The identifier when valid; otherwise null.</param>
    /// <returns><see langword="true"/> when the value is valid.</returns>
    public static bool TryParse(string? value, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out CorrelationId? correlationId)
    {
        if (!string.IsNullOrEmpty(value) && value.Length <= MaxLength && value.All(IsAllowedCharacter))
        {
            correlationId = new CorrelationId(value);
            return true;
        }

        correlationId = null;
        return false;
    }

    /// <inheritdoc />
    public override string ToString() => Value;

    private static bool IsAllowedCharacter(char c)
        => c is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or >= '0' and <= '9' or '-' or '_' or '.';
}
