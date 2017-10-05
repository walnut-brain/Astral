using Astral.Links;

namespace RabbitLink.Services
{
    public interface ILinkCallPublisher<TService, TArg> : IActionPublisher<TService, TArg>
        where TArg : class
    {
        
    }

    public interface ILinkCallPublisher<TService, TArg, TResult> : ICallPublisher<TService, TArg, TResult>
        where TArg : class
        where TResult : class
    {
    }
}