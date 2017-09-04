using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Astral.Contracts;
using Astral.Data;
using Astral.Payloads;
using LanguageExt;

namespace Astral.Deliveries
{
    public interface IDeliveryDataService<TStore>
    {
         Task<(Guid, Payload)> NewDelivery(Type type, object message, DeliveryOperation operation,
            TimeSpan messageTtl, string sponsor, TimeSpan leaseInterval);

        Task CleanSponsorLeases(string sponsor);
        Task<IEnumerable<Guid>> RenewLeases(string sponsor, TimeSpan leaseInterval);
        Task<bool> TryUpdateLease(Guid deliveryId, string sponsor, TimeSpan leaseInterval, TimeSpan? messageTtl);
        Task DeleteDelivery(Guid deliveryId);
        Task RemoveByKey(OperationName operationOperation, string key);
    }
}