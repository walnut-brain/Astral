using Astral.Links;

namespace RabbitLink.Services
{
    public interface ILinkCallConsumer<TService, TArg> : IActionConsumer<TService, TArg>
        where TArg : class
        
    {
        
    }

    public interface ILinkCallConsumer<TService, TArg, TResult> : ICallConsumer<TService, TArg, TResult>
        where TArg : class
        where TResult : class
    {
        
    }
}