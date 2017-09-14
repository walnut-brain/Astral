using Astral;
using RabbitLink.Messaging;

namespace RabbitLink.Astral.Settings
{
    public sealed class ResponseDeliveryModeSetting : Fact<LinkDeliveryMode>
    {
        public ResponseDeliveryModeSetting(LinkDeliveryMode value) : base(value)
        {
        }
    }
}