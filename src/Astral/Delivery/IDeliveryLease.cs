using System;
using System.Threading;
using Astral.Data;

namespace Astral.Delivery
{
    public interface IDeliveryLease<TUoW>
        where TUoW : IUnitOfWork
    {
        CancellationToken Token { get; }
        void Release(Action<IDeliveryDataService<TUoW>> action);
    }
}