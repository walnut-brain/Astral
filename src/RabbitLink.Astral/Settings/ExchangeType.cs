using Astral;
using RabbitLink.Topology;

namespace RabbitLink.Astral.Settings
{
    public sealed class ExchangeType : Fact<LinkExchangeType>
         {
             public ExchangeType(LinkExchangeType value) : base(value)
             {
             }
         }
}