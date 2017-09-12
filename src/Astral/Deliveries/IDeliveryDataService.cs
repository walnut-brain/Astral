using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Astral.Contracts;
using Astral.Payloads;
using Astral.Specifications;

namespace Astral.Deliveries
{
    public interface IDeliveryDataService<TStore>
    {
         

        Task<Payload> NewDelivery<T>(EndpointConfig endpoint, Guid deliveryId, DeliveryReply reply, T message, string sponsor, TimeSpan leaseInterval);
        
        Task CleanSponsorLeases(string sponsor);
        Task<IEnumerable<Guid>> RenewLeases(string sponsor, TimeSpan leaseInterval);
        Task<bool> TryFreeLease(Guid deliveryId, string sponsor, TimeSpan leaseInterval);
        Task DeleteDelivery(Guid deliveryId);

        Task<bool> TryPickupLease(Guid deliveryId, string sponsor, TimeSpan leaseInterval);
    }
}