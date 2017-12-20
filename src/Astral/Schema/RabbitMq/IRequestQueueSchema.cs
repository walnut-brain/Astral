namespace Astral.Schema.RabbitMq
{
    public interface IRequestQueueSchema
    {
        string Name { get; }
        bool Durable { get; }
        bool AutoDelete { get; }
    }
}