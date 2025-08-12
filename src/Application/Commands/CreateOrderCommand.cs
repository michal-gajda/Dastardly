namespace Dastardly.Application.Commands;

using MediatR;

public sealed record class CreateOrderCommand(Guid CustomerId, List<string> Items) : IRequest;
