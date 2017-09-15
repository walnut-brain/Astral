using Astral;
using RabbitLink.Messaging;

namespace RabbitLink.Astral.Settings
{
    public sealed class ResponseDeliveryMode : Fact<LinkDeliveryMode>
    {
        public ResponseDeliveryMode(LinkDeliveryMode value) : base(value)
        {
        }
    }
}