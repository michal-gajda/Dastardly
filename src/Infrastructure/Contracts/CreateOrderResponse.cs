namespace Dastardly.Infrastructure.Contracts;

/// <summary>
/// Published Language: Response when an order is successfully created
/// </summary>
public sealed record CreateOrderResponse
{
    /// <summary>
    /// Unique identifier of the created order
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
    /// Human-readable message about the operation
    /// </summary>
    public required string Message { get; init; }
}
