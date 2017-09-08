using System;
using Astral.Deliveries;

namespace Astral
{
    public class DeliveryOptions
    {
        public DeliveryOptions(DeliveryReplyTo replyTo, TimeSpan? messageTtl = null, DeliveryAfterCommit afterCommit = null)
        {
            ReplyTo = replyTo;
            MessageTtl = messageTtl;
            AfterCommit = afterCommit;
        }

        public TimeSpan? MessageTtl { get; }
        public DeliveryAfterCommit AfterCommit { get; }
        public DeliveryReplyTo ReplyTo { get; }
    }
}