namespace Dastardly.Infrastructure.Messaging;

using Dastardly.Application.Interfaces;
using Hangfire;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

internal sealed class BackgroundCommandDispatcher(IServiceProvider serviceProvider) : IBackgroundCommandDispatcher
{
    public void Enqueue<T>(T command) where T : IRequest
    {
        BackgroundJob.Enqueue(() => ExecuteCommand(command));
    }

    public async Task ExecuteCommand<T>(T command) where T : IRequest
    {
        using var scope = serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send(command);
    }
}
