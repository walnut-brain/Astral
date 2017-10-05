using System.Threading;
using System.Threading.Tasks;

namespace RabbitLink.Services
{
    public interface ICallEndpoint<TService, TArg> 
        where TArg : class
        
    {
        ILinkCallPublisher<TService, TArg> Publisher { get; }
        ILinkCallConsumer<TService, TArg> Consumer { get; }
    }

    
    public interface ICallEndpoint<TService, TArg, TResult>
        where TArg : class
        where TResult : class
    {
        ILinkCallPublisher<TService, TArg, TResult> Publisher { get; }
        ILinkCallConsumer<TService, TArg, TResult> Consumer { get; }
    
    }
}