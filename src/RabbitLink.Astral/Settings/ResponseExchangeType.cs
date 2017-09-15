using Astral;
using RabbitLink.Topology;

namespace RabbitLink.Astral.Settings
{
    public sealed class ResponseExchangeType : Fact<LinkExchangeType>
    {
        public ResponseExchangeType(LinkExchangeType value) : base(value)
        {
        }
    }
}