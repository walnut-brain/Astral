using Astral;
using RabbitLink.Topology;

namespace RabbitLink.Astral.Settings
{
    public sealed class ExchangeTypeSetting : Fact<LinkExchangeType>
         {
             public ExchangeTypeSetting(LinkExchangeType value) : base(value)
             {
             }
         }
}