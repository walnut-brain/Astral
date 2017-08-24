using System.Threading;
using System.Threading.Tasks;

namespace Astral
{
    public interface IEventHandler<in T>
    {
        Task Handle(T @event, EventContext context, CancellationToken token);
    }
}