using System;
using System.Threading;
using Astral.Data;

namespace Astral.Delivery
{
    public interface IDeliveryLease<TStore>
        where TStore : IStore<TStore>
    {
        CancellationToken Token { get; }
        void Release(Action<IDeliveryDataService<TStore>> action);
    }
}