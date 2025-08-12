namespace Dastardly.Infrastructure.Contracts;

/// <summary>
/// API Contract: Response for order query operations
/// </summary>
public sealed record GetOrderResponse
{
    /// <summary>
    /// Unique identifier of the order
    /// </summary>
    public required Guid OrderId { get; init; }

    /// <summary>
    /// Current status of the order
    /// </summary>
    public required string Status { get; init; }

    /// <summary>
    /// Timestamp when the order was created
    /// </summary>
    public required DateTime CreatedAt { get; init; }

    /// <summary>
    /// Timestamp when the order was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; init; }

    /// <summary>
    /// List of items in the order
    /// </summary>
    public required List<string> Items { get; init; } = [];

    /// <summary>
    /// Customer identifier who placed the order
    /// </summary>
    public required Guid CustomerId { get; init; }

    /// <summary>
    /// Human-readable message about the current order state
    /// </summary>
    public required string Message { get; init; }
}
