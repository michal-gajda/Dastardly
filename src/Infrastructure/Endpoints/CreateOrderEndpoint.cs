namespace Dastardly.Infrastructure.Endpoints;

using Dastardly.Application.Commands;
using Dastardly.Application.Interfaces;
using Dastardly.Infrastructure.Contracts;
using Dastardly.Infrastructure.Validators;
using FastEndpoints;
using Microsoft.Extensions.Logging;

/// <summary>
/// API Endpoint: Create Order
/// This endpoint represents the external API contract for order creation
/// It abstracts the internal domain model and provides a stable API contract
/// </summary>
public sealed class CreateOrderEndpoint : Endpoint<CreateOrderRequest, CreateOrderResponse>
{
    private readonly IBackgroundCommandDispatcher _dispatcher;
    private readonly ILogger<CreateOrderEndpoint> _logger;

    public CreateOrderEndpoint(IBackgroundCommandDispatcher dispatcher, ILogger<CreateOrderEndpoint> logger)
    {
        _dispatcher = dispatcher;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/api/orders");
        Summary(s =>
        {
            s.Summary = "Create a new order";
            s.Description = "Creates a new order with the specified items for a customer. This is part of the external API for order management.";
            s.Response(201, "Order created successfully");
            s.Response(400, "Invalid request data");
            s.Response(500, "Internal server error");
        });

        // Add validation rules
        Validator<CreateOrderRequestValidator>();
    }

    public override async Task HandleAsync(CreateOrderRequest request, CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("Processing order creation request for customer {CustomerId} via external API",
                request.CustomerId);

            // Transform external API contract to internal command
            var command = new CreateOrderCommand(request.CustomerId, request.Items);

            // Use background dispatcher for async processing
            _dispatcher.Enqueue(command);

            // Generate response using external API contract
            var response = new CreateOrderResponse
            {
                OrderId = Guid.NewGuid(), // In real implementation, this would come from the command result
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                Message = "Order has been queued for processing"
            };

            _logger.LogInformation("Order creation request queued successfully for customer {CustomerId}",
                request.CustomerId);

            await SendCreatedAtAsync("GetOrder", new { id = response.OrderId }, response, cancellation: ct);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid order creation request for customer {CustomerId}", request.CustomerId);

            await SendErrorsAsync(400, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing order creation request for customer {CustomerId}", request.CustomerId);

            await SendErrorsAsync(500, ct);
        }
    }
}
