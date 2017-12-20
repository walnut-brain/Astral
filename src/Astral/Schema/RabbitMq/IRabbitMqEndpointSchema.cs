namespace Astral.Schema.RabbitMq
{
    public interface IRabbitMqEndpointSchema
    {
        IExchangeSchema Exchange { get; }
        string RoutingKey { get; }
    }
}