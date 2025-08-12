namespace Dastardly.Infrastructure.Mappers;

using Dastardly.Application.Commands;
using Dastardly.Domain.Entities;
using Dastardly.Infrastructure.Contracts;

/// <summary>
/// API Contract Mapper
/// This class handles the translation between the external API contracts
/// and the internal domain model, ensuring the API contract remains stable
/// while allowing internal domain changes
/// </summary>
public static class ApiContractMapper
{
    /// <summary>
    /// Maps an external API CreateOrderRequest to an internal CreateOrderCommand
    /// This abstraction allows the internal command structure to evolve
    /// without breaking the external API contract
    /// </summary>
    public static CreateOrderCommand ToCommand(CreateOrderRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new CreateOrderCommand(request.CustomerId, request.Items);
    }

    /// <summary>
    /// Maps a domain Order entity to an external API CreateOrderResponse
    /// This ensures that internal domain changes don't leak into the API contract
    /// </summary>
    public static CreateOrderResponse ToResponse(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        return new CreateOrderResponse
        {
            OrderId = order.Id,
            Status = order.Status.ToString(),
            CreatedAt = order.CreatedAt,
            Message = GetStatusMessage(order.Status)
        };
    }

    /// <summary>
    /// Maps domain OrderStatus to human-readable messages
    /// This is part of the API contract - status messages that clients can rely on
    /// </summary>
    private static string GetStatusMessage(OrderStatus status) => status switch
    {
        OrderStatus.Pending => "Order has been received and is pending processing",
        OrderStatus.Processing => "Order is currently being processed",
        OrderStatus.Completed => "Order has been completed successfully",
        OrderStatus.Cancelled => "Order has been cancelled",
        _ => "Unknown order status"
    };
}
