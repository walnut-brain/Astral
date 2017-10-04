using System;
using System.Threading;
using System.Threading.Tasks;

namespace Astral.Leasing
{
    public interface ISponsor<out TSponsorId, TResource>
    {
        TSponsorId SponsorId { get; }
        Func<Task> Prepare(TResource resource, Func<CancellationToken, Task> work, ILeaseController<TSponsorId, TResource> controller);
    }
}