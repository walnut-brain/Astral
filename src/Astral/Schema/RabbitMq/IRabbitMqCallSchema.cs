namespace Astral.Schema.RabbitMq
{
    public interface IRabbitMqCallSchema : IRabbitMqEndpointSchema, ICallSchema
    {
        IRequestQueueSchema RequestQueue { get; }
        IExchangeSchema ResponseExchange { get; }
    }
}