using System;
using System.Threading;
using System.Threading.Tasks;

namespace Astral.Links
{
    public interface ICallConsumer<TArg, TResult>
    {
        IDisposable Process(Func<TArg, CancellationToken, Task<TResult>> processor);
    }

    public interface ICallConsumer<TService, TArg, TResult> : ICallConsumer<TArg, TResult>
    {
        
    }
}