using System.Threading;
using System.Threading.Tasks;

namespace Astral.Links
{
    public interface IActionPublisher<TArg>
    {
        Task Call(TArg arg, CancellationToken token);
    }

    public interface IActionPublisher<TService, TArg> : IActionPublisher<TArg>
    {
    }
}