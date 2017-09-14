using Astral;
using RabbitLink.Messaging;

namespace RabbitLink.Astral.Settings
{
    public sealed class DeliveryModeSetting : Fact<LinkDeliveryMode>
    {
        public DeliveryModeSetting(LinkDeliveryMode value) : base(value)
        {
        }
    }
}