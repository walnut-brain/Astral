using System;

namespace Astral.Data
{
    public class DeliveryRecord
    {
        public DeliveryRecord(Guid deliveryId, string serviceName, string endpointName, string contractCode,
            string encoding, string body, string key, DateTimeOffset leasedTo, string sponsor, bool delivered,
            bool needAnswer, DateTimeOffset? ttl)
        {
            DeliveryId = deliveryId;
            ServiceName = serviceName;
            EndpointName = endpointName;
            ContractCode = contractCode;
            Encoding = encoding;
            Body = body;
            Key = key;
            LeasedTo = leasedTo;
            Sponsor = sponsor;
            Delivered = delivered;
            NeedAnswer = needAnswer;
            Ttl = ttl;
        }

        public Guid DeliveryId { get; }
        public string ServiceName { get; }
        public string EndpointName { get; }
        public string ContractCode { get; }
        public string Encoding { get; }
        public string Body { get; }
        public string Key { get; }
        public DateTimeOffset LeasedTo { get; }
        public string Sponsor { get; }
        public bool Delivered { get; }
        public bool NeedAnswer { get; }
        public DateTimeOffset? Ttl { get; }
    }
}