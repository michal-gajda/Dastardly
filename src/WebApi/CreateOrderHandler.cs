namespace Dastardly.WebApi;

using MediatR;

public sealed class CreateOrderHandler : IRequestHandler<CreateOrderCommand>
{
    public async Task Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        if (request.Items.Count == 0)
        {
            throw new InvalidOperationException("Order must contain at least one item.");
        }

        Console.WriteLine($"[HANDLER] Creating order for Customer: {request.CustomerId}");

        await Task.Delay(100);
    }
}
