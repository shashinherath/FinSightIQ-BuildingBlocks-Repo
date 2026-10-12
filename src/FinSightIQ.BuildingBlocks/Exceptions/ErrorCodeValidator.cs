using System.Text.RegularExpressions;

namespace FinSightIQ.BuildingBlocks.Exceptions;

/// <summary>
/// Validates the <c>UPPER_SNAKE_CASE</c> error-code convention from the FinSightIQ API documentation.
/// </summary>
internal static partial class ErrorCodeValidator
{
    private const int MaxLength = 100;

    [GeneratedRegex("^[A-Z][A-Z0-9]*(_[A-Z0-9]+)*$", RegexOptions.CultureInvariant)]
    private static partial Regex CodePattern();

    internal static void Validate(string? code, string paramName)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Error code must not be null or blank.", paramName);
        }

        if (code.Length > MaxLength || !CodePattern().IsMatch(code))
        {
            throw new ArgumentException(
                $"Error code must be UPPER_SNAKE_CASE and at most {MaxLength} characters.",
                paramName);
        }
    }
}
