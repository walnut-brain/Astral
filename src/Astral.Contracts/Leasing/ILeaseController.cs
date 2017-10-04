using System;
using System.Threading.Tasks;

namespace Astral.Leasing
{
    public interface ILeaseController<in TSponsorId, in TResource>
    {
        Task RenewLease(TSponsorId sponsor, TResource resource, TimeSpan leaseTime);
        Task FreeLease(TSponsorId sponsorId, TResource resource, Exception exception = null);
    }
}