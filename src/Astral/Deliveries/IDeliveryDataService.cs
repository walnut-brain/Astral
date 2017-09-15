using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Astral.Data;
using Astral.Payloads;
using Astral.Specifications;

namespace Astral.Deliveries
{
    public interface IDeliveryDataService<TStore> : IStoreService<TStore>
    {
         

        Task<Payload> NewDelivery<T>(EndpointConfig endpoint, Guid deliveryId, DeliveryReply reply, T message, string sponsor, TimeSpan leaseInterval);
        
        Task CleanSponsorLeases(string sponsor);
        Task<IEnumerable<Guid>> RenewLeases(string sponsor, TimeSpan leaseInterval);
        Task<bool> TryFreeLease(Guid deliveryId, string sponsor, TimeSpan leaseInterval);
        Task DeleteDelivery(Guid deliveryId);

        Task<bool> TryPickupLease(Guid deliveryId, string sponsor, TimeSpan leaseInterval);

        Task<IEnumerable<Guid>> RenewSponsor(string sponsorName, TimeSpan leaseInterval, 
            IEnumerable<Guid> completeDeliveries, IEnumerable<Guid> archiveDeliveries);

        Task<Payload> AddDelivery(string sponsorName, Guid deliveryId, string service, string endpoint, string sender,
            ChannelKind.ReplyChannel replyChannel);

    }

    
}