using RabbitLink.Topology;

namespace RabbitLink.Services.Descriptions
{
    public class ExchangeDescription
    {
        public ExchangeDescription(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public LinkExchangeType Type { get; set; } = LinkExchangeType.Direct;
        public bool Durable { get; set; } = true;
        public bool AutoDelete { get; set; } = false;
        public string Alternate { get; set; }
        public bool Delayed { get; set; } = false;
    }
}