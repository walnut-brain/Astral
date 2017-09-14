using Astral;
using RabbitLink.Topology;

namespace RabbitLink.Astral.Settings
{
    public sealed class ResponseExchangeTypeSetting : Fact<LinkExchangeType>
    {
        public ResponseExchangeTypeSetting(LinkExchangeType value) : base(value)
        {
        }
    }
}