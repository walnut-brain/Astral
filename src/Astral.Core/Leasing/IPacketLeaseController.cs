using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Astral.Leasing
{
    public interface IPacketLeaseController<TResource> : ILeaseController<TResource>
    {
        Task<IEnumerable<TResource>> RenewLeases(string sponsor, TimeSpan leaseTime);
    }
}