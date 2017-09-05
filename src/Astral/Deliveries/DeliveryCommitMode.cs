using System;

namespace Astral.Deliveries
{
    public abstract class DeliveryCommitMode
    {
        private DeliveryCommitMode()
        {
            
        }

        public sealed class OptimisticType : DeliveryCommitMode
        {
            public TimeSpan AwaitPeriod { get; }
            public OptimisticType(TimeSpan awaitPeriod) 
            {
                AwaitPeriod = awaitPeriod;
            }
        }
        public sealed class PessimisticType : DeliveryCommitMode
        {
            
        }

        public static DeliveryCommitMode Optimistic(TimeSpan awaitPeriod)
            => new OptimisticType(awaitPeriod);
        public static DeliveryCommitMode Pessimistic = new PessimisticType();
    }
}