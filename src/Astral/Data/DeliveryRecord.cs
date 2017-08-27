using System;
using Astral.Serialization;

namespace Astral.Data
{
    public class DeliveryRecord
    {
        public DeliveryRecord(Guid deliveryId, string serviceName, string endpointName, Serialized<string> serialized, DateTimeOffset leasedTo)
        {
            DeliveryId = deliveryId;
            ServiceName = serviceName;
            EndpointName = endpointName;
            LeasedTo = leasedTo;
            Serialized = serialized;
        }

        public Guid DeliveryId { get; }
        public string ServiceName { get; }
        public string EndpointName { get; }
        public Guid? CorrelationId { get; set; }
        public string ReplayTo { get; set; }
        public Serialized<string> Serialized { get; }
        public string Key { get; set; }
        public DateTimeOffset LeasedTo { get; }
        public string Sponsor { get; set; }
        public bool Delivered { get; set; } = false;
        public bool NeedAnswer { get; set; } = false;
        public DateTimeOffset? Ttl { get; set; }
        public int Attempt { get; set; } = 0;
        public string LastError { get; set; }

        public bool IsAnswer => CorrelationId.HasValue && !string.IsNullOrWhiteSpace(ReplayTo);
    }
}