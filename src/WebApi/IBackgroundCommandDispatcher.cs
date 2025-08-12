namespace Dastardly.WebApi;

using MediatR;

public interface IBackgroundCommandDispatcher
{
    void Enqueue<T>(T command) where T : IRequest;
}
