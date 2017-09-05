using System;
using System.Security.Permissions;
using Astral.Contracts;

namespace Astral.Deliveries
{
    public abstract class DeliveryOperation
    {
        public SystemName System { get; }
        public TransportTag Transport { get; }
        public OperationName Operation { get; }

        public class Send : DeliveryOperation
        {
            
        }

        public class SendWithReply : DeliveryOperation
        {
            public DeliveryReplyTo ReplyTo { get; }
        }

        public class Reply : DeliveryOperation
        {
            public DeliveryReplyTo ReplyTo { get; }    
            public string RequestCorrelationId { get; }
        }
    }
}