using FinSightIQ.BuildingBlocks.Exceptions;

namespace FinSightIQ.BuildingBlocks.Results;

/// <summary>
/// Structured description of an expected failure.
/// </summary>
/// <remarks>
/// <see cref="Code"/> is part of the API contract and must be stable (for example
/// <c>SYMBOL_NOT_FOUND</c>). <see cref="Message"/> is for humans and may change.
/// Do not place sensitive data in the message.
/// </remarks>
public sealed record Error
{
    /// <summary>Gets the stable, machine-readable <c>UPPER_SNAKE_CASE</c> error code.</summary>
    public string Code { get; }

    /// <summary>Gets the human-readable description.</summary>
    public string Message { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> record.
    /// </summary>
    /// <param name="code">A non-blank <c>UPPER_SNAKE_CASE</c> code.</param>
    /// <param name="message">A non-blank description.</param>
    /// <exception cref="ArgumentException">The code or message is blank, or the code is malformed.</exception>
    public Error(string code, string message)
    {
        ErrorCodeValidator.Validate(code, nameof(code));
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        Code = code;
        Message = message;
    }
}
