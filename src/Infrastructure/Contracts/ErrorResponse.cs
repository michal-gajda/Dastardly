namespace Dastardly.Infrastructure.Contracts;

/// <summary>
/// Published Language: Error response for failed operations
/// </summary>
public sealed record ErrorResponse
{
    /// <summary>
    /// Error code for programmatic handling
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// Human-readable error message
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// Additional details about the error
    /// </summary>
    public string? Details { get; init; }

    /// <summary>
    /// Timestamp when the error occurred
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
