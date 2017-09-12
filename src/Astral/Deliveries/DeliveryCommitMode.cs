using System;

namespace Astral.Deliveries
{
    public struct DeliveryCommitMode
    {
        private readonly TimeSpan? _awaitPeriod;
        
        private DeliveryCommitMode(TimeSpan period)
        {
            _awaitPeriod = period;
        }

        public static DeliveryCommitMode Optimistic(TimeSpan awaitPeriod)
            => new DeliveryCommitMode(awaitPeriod);
        public static readonly DeliveryCommitMode Pessimistic = default(DeliveryCommitMode);
    }
}