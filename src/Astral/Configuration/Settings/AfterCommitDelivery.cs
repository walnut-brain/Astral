using Astral.Deliveries;

namespace Astral.Configuration.Settings
{
    public sealed class AfterCommitDelivery : Fact<DeliveryAfterCommit>
    {
        public AfterCommitDelivery(DeliveryAfterCommit value) : base(value)
        {
        }
    }
}