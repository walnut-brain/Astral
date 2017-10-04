using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Astral.Leasing
{
    public interface IPacketLeaseController<in TSponsorId, TResource> : ILeaseController<TSponsorId, TResource>
    {
        Task<IEnumerable<TResource>> RenewLeases(TSponsorId sponsor, TimeSpan leaseTime);
    }
}