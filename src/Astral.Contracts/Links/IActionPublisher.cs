using System;
using System.Threading;
using System.Threading.Tasks;

namespace Astral.Links
{
    public interface IActionPublisher<TArg>
    {
        Task Call(TArg arg, CancellationToken token = default (CancellationToken));
        Task Call(TArg arg, TimeSpan timeout);
    }

    public interface IActionPublisher<TService, TArg> : IActionPublisher<TArg>
    {
    }
}