using Astral.Links;
using RabbitLink.Consumer;

namespace RabbitLink.Services
{
    public interface ILinkEventConsumer<TService, TEvent> : IEventConsumer<TService, TEvent>
        where TEvent : class
    {
        ILinkEventConsumer<TService, TEvent> Queue(string queueName);
        ILinkEventConsumer<TService, TEvent> PrefetchCount(ushort value);
        ILinkEventConsumer<TService, TEvent> AutoAck(bool value);
        ILinkEventConsumer<TService, TEvent> ErrorStrategy(ILinkConsumerErrorStrategy value);
        ILinkEventConsumer<TService, TEvent> CancelOnHaFailover(bool value);
        ILinkEventConsumer<TService, TEvent> Exclusive(bool value);
        ILinkEventConsumer<TService, TEvent> ExchangePassive(bool value);
        ILinkEventConsumer<TService, TEvent> QueuePassive(bool value);
        ILinkEventConsumer<TService, TEvent> Bind(bool value);
        ILinkEventConsumer<TService, TEvent> QueueParameters(QueueParameters parameters);
        ILinkEventConsumer<TService, TEvent> AddRoutingKey(string value);
        ILinkEventConsumer<TService, TEvent> AddRoutingKeyByExample(TEvent value);
    }
}