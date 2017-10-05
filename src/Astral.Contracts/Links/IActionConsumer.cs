using System;
using System.Threading;
using System.Threading.Tasks;

namespace Astral.Links
{
    public interface IActionConsumer<TArg>
    {
        IDisposable Process(Func<TArg, CancellationToken, Task> processor);
    }

    public interface IActionConsumer<TService, TArg> : IActionConsumer<TArg>
    {
        
    }
}