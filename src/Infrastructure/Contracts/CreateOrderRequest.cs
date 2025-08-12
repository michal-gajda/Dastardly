namespace Dastardly.Infrastructure.Contracts;

/// <summary>
/// Published Language: Request to create a new order in the system
/// </summary>
public sealed record CreateOrderRequest
{
    /// <summary>
    /// Unique identifier of the customer placing the order
    /// </summary>
    public required Guid CustomerId { get; init; }

    /// <summary>
    /// List of item names to include in the order
    /// </summary>
    public required List<string> Items { get; init; } = [];
}
