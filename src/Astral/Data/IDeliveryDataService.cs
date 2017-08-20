using System;
using System.Collections.Generic;

namespace Astral.Data
{
    public interface IDeliveryDataService<T> : IDataService<T> 
        where T : IUnitOfWork

    {
        void DeleteAll(string serviceName, string endpointName, string messageKey);

        void Create(DeliveryRecord delivery);

        bool UpdateLease(string sponsor, Guid deliveryId, DateTimeOffset leasedTo);
        void SetDelivered(Guid deliveryId, DateTimeOffset archiveTo);
        void Delete(Guid deliveryId);

        IEnumerable<DeliveryRecord> GetAwaitedDelivery(string serviceName, string endpointName, string sponsorName,
            DateTimeOffset leaseTo, uint? maxCount);

        void CleanLateDeliveries();
    }
}