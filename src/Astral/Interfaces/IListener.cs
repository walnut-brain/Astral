using System.Threading;
using System.Threading.Tasks;

namespace Astral
{
    public interface IListener<in T, in TContext>
    {
        Task Handle(T @message, TContext context, CancellationToken token);
    }
}