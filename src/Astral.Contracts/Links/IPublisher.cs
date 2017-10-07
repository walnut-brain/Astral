using System.Threading;
using System.Threading.Tasks;

namespace Astral.Links
{
    public interface IPublisher<in TEvent> 
    {
        Task PublishAsync(TEvent message, CancellationToken token = default(CancellationToken));
    }

    public interface IPublisher<TService, in TEvent> : IPublisher<TEvent>
    {
        
    }
}