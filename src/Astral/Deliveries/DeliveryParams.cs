using System;

namespace Astral.Deliveries
{
    internal class DeliveryParams
    {
        public DeliveryParams(DeliveryOperation operation, DeliveryAfterCommit afterCommit) : this(operation, afterCommit, new TimeSpan())
        {
        }

        public DeliveryParams(DeliveryOperation operation, DeliveryAfterCommit afterCommit, TimeSpan messageTtl)
        {
            Operation = operation ?? throw new ArgumentNullException(nameof(operation));
            AfterCommit = afterCommit ?? throw new ArgumentNullException(nameof(afterCommit));
            MessageTtl = messageTtl;
        }

        public DeliveryOperation Operation { get; }
        public DeliveryAfterCommit AfterCommit { get; }
        public TimeSpan MessageTtl { get; }
        
    }
}