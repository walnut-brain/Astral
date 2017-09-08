using System;
using System.Security.Permissions;
using Astral.Configuration.Settings;
using Astral.Contracts;
using Astral.Transport;

namespace Astral.Deliveries
{
    public abstract class DeliveryOperation
    {
        private DeliveryOperation()
        {
        }
        
        public abstract ResponseTo ResponseTo { get; } 

        public class SendType : DeliveryOperation
        { 
            internal SendType()
            {
            }

            public override ResponseTo ResponseTo => ResponseTo.None;
        }

        public class SendWithReplyType : DeliveryOperation
        {
            internal SendWithReplyType(DeliveryReplyTo replyTo) 
            {
                ReplyTo = replyTo;
            }

            public DeliveryReplyTo ReplyTo { get; }

            public override ResponseTo ResponseTo
            {
                get
                {
                    switch (ReplyTo)
                    {
                        case DeliveryReplyTo.SubsystemType subsystem:
                            return ResponseTo.Named(subsystem.Name);
                        case DeliveryReplyTo.SystemType system:
                            return ResponseTo.System;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }
        }

        public class ReplyType : DeliveryOperation
        {
            internal ReplyType(DeliveryReplyTo replyTo, string requestCorrelationId) 
            {
                ReplyTo = replyTo;
                RequestCorrelationId = requestCorrelationId;
            }

            public DeliveryReplyTo ReplyTo { get; }    
            public string RequestCorrelationId { get; }

            public override ResponseTo ResponseTo => ResponseTo.None;
        }

        public static readonly DeliveryOperation Send = new SendType();
        public static DeliveryOperation SendWithReply(DeliveryReplyTo replyTo) => new SendWithReplyType(replyTo);

        public static DeliveryOperation Reply(DeliveryReplyTo replyTo, string requestCorrelationId)
            => new ReplyType(replyTo, requestCorrelationId);
        
        
    }
}