using System.Threading;
using System.Threading.Tasks;

namespace Astral.Links
{
    public interface IEventPublisher<in TEvent> 
    {
        Task PublishAsync(TEvent message, CancellationToken token = default(CancellationToken));
    }

    public interface IEventPublisher<TService, in TEvent> : IEventPublisher<TEvent>
    {
        
    }
}