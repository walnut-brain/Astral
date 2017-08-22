using System;

namespace Astral.Data
{
    public class DeliveryRecord
    {
        public DeliveryRecord(Guid deliveryId, string serviceName, string endpointName, string contractCode,
            string encoding, string body, string key, DateTimeOffset leasedTo, string sponsor, bool delivered,
            bool needAnswer, DateTimeOffset? ttl, Guid? correlationId, string replayTo, int attempt, string lastError)
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
            CorrelationId = correlationId;
            ReplayTo = replayTo;
            Attempt = attempt;
            LastError = lastError;
        }

        public Guid DeliveryId { get; }
        public string ServiceName { get; }
        public string EndpointName { get; }
        public Guid? CorrelationId  { get; }
        public string ReplayTo { get; }
        public string ContractCode { get; }
        public string Encoding { get; }
        public string Body { get; }
        public string Key { get; }
        public DateTimeOffset LeasedTo { get; }
        public string Sponsor { get; }
        public bool Delivered { get; }
        public bool NeedAnswer { get; }
        public DateTimeOffset? Ttl { get; }
        public int Attempt { get; }
        public string LastError { get; }

        public bool IsAnswer => CorrelationId.HasValue && !string.IsNullOrWhiteSpace(ReplayTo);
    }
}