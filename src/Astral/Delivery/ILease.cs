using System;
using System.Threading;
using System.Threading.Tasks;
using Astral.Data;

namespace Astral.Delivery
{
    public interface ILease<TStore> : IDisposable
        where TStore : IStore<TStore>
    {
        LeaseState State { get; }
        CancellationToken Token { get; }
        Task Release(ReleaseAction action);
    }
}