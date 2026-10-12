namespace FinSightIQ.BuildingBlocks.Exceptions;

/// <summary>
/// Signals a violated domain invariant or unrecoverable business-rule breach.
/// </summary>
/// <remarks>
/// Prefer <see cref="Results.Result{T}"/> for expected failures. The exception message must not
/// contain sensitive data. This type is independent of HTTP; mapping <see cref="Code"/> to a status
/// code is the job of the web layer.
/// </remarks>
public class DomainException : Exception
{
    /// <summary>Initializes a new instance of the <see cref="DomainException"/> class.</summary>
    /// <param name="code">A stable <c>UPPER_SNAKE_CASE</c> error code.</param>
    /// <param name="message">A non-blank, non-sensitive description.</param>
    /// <exception cref="ArgumentException">The code or message is invalid.</exception>
    public DomainException(string code, string message)
        : base(ValidateMessage(message))
    {
        ErrorCodeValidator.Validate(code, nameof(code));
        Code = code;
    }

    /// <summary>Initializes a new instance that wraps another exception.</summary>
    /// <param name="code">A stable <c>UPPER_SNAKE_CASE</c> error code.</param>
    /// <param name="message">A non-blank, non-sensitive description.</param>
    /// <param name="innerException">The wrapped exception, preserved as <see cref="Exception.InnerException"/>.</param>
    /// <exception cref="ArgumentException">The code or message is invalid.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="innerException"/> is null.</exception>
    public DomainException(string code, string message, Exception innerException)
        : base(ValidateMessage(message), innerException ?? throw new ArgumentNullException(nameof(innerException)))
    {
        ErrorCodeValidator.Validate(code, nameof(code));
        Code = code;
    }

    /// <summary>Gets the stable, machine-readable error code.</summary>
    public string Code { get; }

    private static string ValidateMessage(string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        return message;
    }
}
