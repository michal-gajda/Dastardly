namespace Dastardly.Application.Commands;

using Dastardly.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

public sealed class CreateOrderHandler : IRequestHandler<CreateOrderCommand>
{
    private readonly ILogger<CreateOrderHandler> _logger;

    public CreateOrderHandler(ILogger<CreateOrderHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating order for Customer: {CustomerId} with {ItemCount} items",
            request.CustomerId, request.Items.Count);

        // Create domain entity
        var order = new Order(request.CustomerId, request.Items);

        // Here you would typically save to repository
        // await _orderRepository.AddAsync(order, cancellationToken);

        _logger.LogInformation("Order {OrderId} created successfully", order.Id);

        // Simulate some work
        await Task.Delay(100, cancellationToken);
    }
}
