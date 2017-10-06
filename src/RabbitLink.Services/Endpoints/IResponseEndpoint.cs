using Astral.Links;

namespace RabbitLink.Services
{
    public interface IResponseEndpoint<TService, TRequest, TResponse> : IEventPublisher<TService, Response<TResponse>>, IEventConsumer<TService, Request<TRequest>>
    {
        
    }
}