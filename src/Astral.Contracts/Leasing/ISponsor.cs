using System;
using System.Threading;
using System.Threading.Tasks;

namespace Astral.Leasing
{
    public interface ISponsor<TResource, TController>
        where TController : ILeaseController<TResource>
    {
        string SponsorName { get; }
        Func<Task> Prepare(TResource resource, Func<CancellationToken, Task> work);
    }
}