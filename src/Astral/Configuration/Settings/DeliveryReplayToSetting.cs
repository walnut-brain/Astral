using Astral.Deliveries;

namespace Astral.Configuration.Settings
{
    public sealed class DeliveryReplayToSetting :Fact<DeliveryReplyTo> 
    {
        public DeliveryReplayToSetting(DeliveryReplyTo value) : base(value)
        {
        }
    }
}