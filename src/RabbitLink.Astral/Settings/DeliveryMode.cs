using Astral;
using RabbitLink.Messaging;

namespace RabbitLink.Astral.Settings
{
    public sealed class DeliveryMode : Fact<LinkDeliveryMode>
    {
        public DeliveryMode(LinkDeliveryMode value) : base(value)
        {
        }
    }
}