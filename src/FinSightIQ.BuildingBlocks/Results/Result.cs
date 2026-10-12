namespace FinSightIQ.BuildingBlocks.Results;

/// <summary>
/// Outcome of an operation that returns no value. Use for expected business failures
/// instead of exceptions.
/// </summary>
public class Result
{
    private readonly Error? _error;

    /// <summary>Initializes the result; success and error must not contradict each other.</summary>
    /// <param name="isSuccess">Whether the operation succeeded.</param>
    /// <param name="error">The error; required when failed and forbidden when successful.</param>
    protected Result(bool isSuccess, Error? error)
    {
        if (isSuccess && error is not null)
        {
            throw new ArgumentException("A successful result cannot contain an error.", nameof(error));
        }

        if (!isSuccess && error is null)
        {
            throw new ArgumentException("A failed result must contain an error.", nameof(error));
        }

        IsSuccess = isSuccess;
        _error = error;
    }

    /// <summary>Gets a value indicating whether the operation succeeded.</summary>
    public bool IsSuccess { get; }

    /// <summary>Gets a value indicating whether the operation failed.</summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>Gets the error of a failed result.</summary>
    /// <exception cref="InvalidOperationException">The result is successful.</exception>
    public Error Error => _error
        ?? throw new InvalidOperationException("A successful result has no error.");

    /// <summary>Creates a successful result.</summary>
    public static Result Success() => new(true, null);

    /// <summary>Creates a failed result.</summary>
    /// <exception cref="ArgumentNullException"><paramref name="error"/> is null.</exception>
    public static Result Failure(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return new Result(false, error);
    }

    /// <summary>Creates a failed result from a code and message.</summary>
    /// <exception cref="ArgumentException">The code or message is invalid.</exception>
    public static Result Failure(string code, string message) => Failure(new Error(code, message));
}

/// <summary>
/// Outcome of an operation that returns a <typeparamref name="T"/> on success.
/// </summary>
/// <typeparam name="T">The value type.</typeparam>
public sealed class Result<T> : Result
{
    private readonly T? _value;

    private Result(T? value, Error? error)
        : base(error is null, error)
    {
        _value = value;
    }

    /// <summary>Gets the value of a successful result.</summary>
    /// <exception cref="InvalidOperationException">The result is a failure.</exception>
    public T Value => IsSuccess
        ? _value!  // Non-null guaranteed: Success() rejects null values.
        : throw new InvalidOperationException("A failed result has no value.");

    /// <summary>Creates a successful result.</summary>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
    public static Result<T> Success(T value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new Result<T>(value, null);
    }

    /// <summary>Creates a failed result.</summary>
    /// <exception cref="ArgumentNullException"><paramref name="error"/> is null.</exception>
    public static new Result<T> Failure(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return new Result<T>(default, error);
    }

    /// <summary>Creates a failed result from a code and message.</summary>
    /// <exception cref="ArgumentException">The code or message is invalid.</exception>
    public static new Result<T> Failure(string code, string message) => Failure(new Error(code, message));

    /// <summary>Attempts to read the value without throwing.</summary>
    /// <param name="value">The value when successful; otherwise default.</param>
    /// <returns><see langword="true"/> when the result is successful.</returns>
    public bool TryGetValue(out T value)
    {
        value = IsSuccess ? _value! : default!;
        return IsSuccess;
    }
}
