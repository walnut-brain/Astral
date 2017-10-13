using Astral.Markup.RabbitMq;

namespace Astral.Schema.RabbitMq
{
    public interface IExchangeSchema
    {
        string Name { get; }
        ExchangeKind Type { get; }
        bool Durable { get; }
        bool AutoDelete { get; }
        bool Delayed { get; }
        string Alternate { get; }
    }
}