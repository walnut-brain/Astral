using System;
using System.Security.Permissions;
using Astral.Configuration.Settings;
using Astral.Contracts;

namespace Astral.Deliveries
{
    public abstract class DeliveryOperation
    {
        private DeliveryOperation()
        {
        }

        public class SendType : DeliveryOperation
        { 
            internal SendType()
            {
            }
        }

        public class SendWithReplyType : DeliveryOperation
        {
            internal SendWithReplyType(DeliveryReplyTo replyTo) 
            {
                ReplyTo = replyTo;
            }

            public DeliveryReplyTo ReplyTo { get; }
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
        }

        public static readonly DeliveryOperation Send = new SendType();
        public static DeliveryOperation SendWithReply(DeliveryReplyTo replyTo) => new SendWithReplyType(replyTo);

        public static DeliveryOperation Reply(DeliveryReplyTo replyTo, string requestCorrelationId)
            => new ReplyType(replyTo, requestCorrelationId);
    }
}