using System;
using Astral.Payloads;
using Astral.Serialization;

namespace Astral.Data
{
    public class DeliveryRecord
    {
        public DeliveryRecord(Guid deliveryId, string serviceName, string endpointName, PayloadBase<string> payload, DateTimeOffset leasedTo)
        {
            DeliveryId = deliveryId;
            ServiceName = serviceName;
            EndpointName = endpointName;
            LeasedTo = leasedTo;
            PayloadBase = payload;
        }

        public Guid DeliveryId { get; }
        public string ServiceName { get; }
        public string EndpointName { get; }
        public Guid? CorrelationId { get; set; }
        public string ReplayTo { get; set; }
        public PayloadBase<string> PayloadBase { get; }
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