namespace Dastardly.Infrastructure.Endpoints;

using Dastardly.Infrastructure.Contracts;
using FastEndpoints;
using Microsoft.Extensions.Logging;

/// <summary>
/// API Endpoint: Get Order Status
/// This endpoint allows clients to query order status using the external API
/// </summary>
public sealed class GetOrderEndpoint : EndpointWithoutRequest<GetOrderResponse>
{
    private readonly ILogger<GetOrderEndpoint> _logger;

    public GetOrderEndpoint(ILogger<GetOrderEndpoint> logger)
    {
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/api/orders/{id}");
        Summary(s =>
        {
            s.Summary = "Get order by ID";
            s.Description = "Retrieves order information by order ID. This is part of the external API for order management.";
            s.Response(200, "Order found");
            s.Response(404, "Order not found");
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var orderId = Route<Guid>("id");

        _logger.LogInformation("Retrieving order {OrderId} via external API", orderId);

        // In real implementation, this would query the read model/repository
        // For now, return a mock response
        var response = new GetOrderResponse
        {
            OrderId = orderId,
            Status = "Processing",
            CreatedAt = DateTime.UtcNow.AddMinutes(-5),
            UpdatedAt = DateTime.UtcNow.AddMinutes(-2),
            Items = ["Widget A", "Widget B"],
            CustomerId = Guid.NewGuid(),
            Message = "Order is being processed"
        };

        await SendOkAsync(response, ct);
    }
}
