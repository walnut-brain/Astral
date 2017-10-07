using System;
using System.Threading;
using System.Threading.Tasks;

namespace Astral.Links
{
    public interface ICallPublisher<TArg, TResult>
    {
        Task<TResult> Call(TArg arg, CancellationToken token = default(CancellationToken));
        Task<TResult> Call(TArg arg, TimeSpan timeout);
    }
    
    public interface ICallPublisher<TService, TArg, TResult> : ICallPublisher<TArg, TResult>
    {
        
    }
}