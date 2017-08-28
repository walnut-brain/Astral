using System;

namespace Astral.Transport
{
    public class PublishOptions
    {
        public PublishOptions(TimeSpan messageTtl, ResponseTo responseTo, Guid? correlationId)
        {
            MessageTtl = messageTtl;
            ResponseTo = responseTo;
            CorrelationId = correlationId;
        }

        public TimeSpan MessageTtl { get; }
        public ResponseTo ResponseTo { get; }
        public Guid? CorrelationId { get; }
    }
}