using System;
using System.Threading.Tasks;

namespace Astral.Leasing
{
    public interface ILeaseController<in TResource>
    {
        Task RenewLease(string sponsor, TResource resource, TimeSpan leaseTime);
        Task FreeLease(string sponsor, TResource resource, Exception exception = null);
    }
}