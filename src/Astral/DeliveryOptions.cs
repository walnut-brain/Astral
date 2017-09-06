using System;
using Astral.Deliveries;

namespace Astral
{
    public class DeliveryOptions
    {
        public DeliveryOptions(TimeSpan? messageTtl = null, DeliveryAfterCommit afterCommit = null)
        {
            MessageTtl = messageTtl;
            AfterCommit = afterCommit;
        }

        public TimeSpan? MessageTtl { get; }
        public DeliveryAfterCommit AfterCommit { get; }
    }
}