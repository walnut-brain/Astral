using System;

namespace Astral.Transport
{
    public class PublishOptions
    {
        public PublishOptions(TimeSpan messageTtl, ChannelKind.IResponseTo responseTo, string correlationId)
        {
            MessageTtl = messageTtl;
            ResponseTo = (ChannelKind) responseTo;
            CorrelationId = correlationId;
        }

        public TimeSpan MessageTtl { get; }
        public ChannelKind ResponseTo { get; }
        public string CorrelationId { get; }
    }
}