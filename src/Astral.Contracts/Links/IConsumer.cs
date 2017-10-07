using System;
using System.Threading;
using System.Threading.Tasks;

namespace Astral.Links
{
    public interface IConsumer<out TEvent> 
    {
        IDisposable Listen(Func<TEvent, CancellationToken, Task<Acknowledge>> listener);
    }
    
    public interface IConsumer<TService, out TEvent> : IConsumer<TEvent>
    {
        
    }
}