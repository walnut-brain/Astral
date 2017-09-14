using RabbitLink.Topology;

namespace RabbitLink.Astral
{
    internal class ExchangeConfig
    {
        public ExchangeConfig(string name, 
            LinkExchangeType type, 
            bool asPassive, 
            bool durable, 
            bool confirmsMode)
        {
            Name = name;
            Type = type;
            AsPassive = asPassive;
            Durable = durable;
            ConfirmsMode = confirmsMode;
        }

        public string Name { get; }
        public LinkExchangeType Type { get; }
        public bool AsPassive { get; }
        public bool Durable { get; }
        public bool ConfirmsMode { get; }
    }
}