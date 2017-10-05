using System.Threading;
using System.Threading.Tasks;

namespace Astral.Links
{
    public interface ICallPublisher<TArg, TResult>
    {
        Task<TResult> Call(TArg arg, CancellationToken token);
    }
    
    public interface ICallPublisher<TService, TArg, TResult> : ICallPublisher<TArg, TResult>
    {
        
    }
}