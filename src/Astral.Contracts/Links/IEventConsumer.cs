using System;
using System.Threading;
using System.Threading.Tasks;

namespace Astral.Links
{
    public interface IEventConsumer
    {
        IDisposable Listen(Func<object, CancellationToken, Task<Acknowledge>> listener);
    }

    public interface IEventConsumer<out TEvent> 
    {
        IDisposable Listen(Func<TEvent, CancellationToken, Task<Acknowledge>> listener);
    }
    
    public interface IEventConsumer<TService, out TEvent> : IEventConsumer<TEvent>
    {
        
    }
}