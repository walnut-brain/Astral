using System;
using System.Collections.Generic;
using System.Net.Mime;
using Astral.Links;
using RabbitLink.Consumer;

namespace RabbitLink.Services
{
    public interface IEventEndpoint<TService, TEvent> : IEventConsumer<TService, TEvent>, IEventPublisher<TService, TEvent>
    {
        ContentType ContentType { get; }
        bool ExchangePassive();
        IEventEndpoint<TService, TEvent> ExchangePassive(bool value);
        string ExchangeNamed();
        IEventEndpoint<TService, TEvent> ExchangeNamed(string value);
        bool ConfirmsMode();
        IEventEndpoint<TService, TEvent> ConfirmsMode(bool value);
        string QueueName();
        IEventEndpoint<TService, TEvent> QueueName(string value);
        ushort PrefetchCount();
        IEventEndpoint<TService, TEvent> PrefetchCount(ushort value);
        bool AutoAck();
        IEventEndpoint<TService, TEvent> AutoAck(bool value);
        ILinkConsumerErrorStrategy ErrorStrategy();
        IEventEndpoint<TService, TEvent> ErrorStrategy(ILinkConsumerErrorStrategy value);
        bool? CancelOnHaFailover();
        IEventEndpoint<TService, TEvent> CancelOnHaFailover(bool? value);
        bool Exclusive();
        IEventEndpoint<TService, TEvent> Exclusive(bool value);
        bool QueuePassive();
        IEventEndpoint<TService, TEvent> QueuePassive(bool value);
        bool Bind();
        IEventEndpoint<TService, TEvent> Bind(bool value);
        QueueParameters QueueParameters();
        IEventEndpoint<TService, TEvent> QueueParameters(Func<QueueParameters, QueueParameters> setter);
        IEnumerable<string> RoutingKeys();
        IEventEndpoint<TService, TEvent> AddRoutingKey(string value);
        IEventEndpoint<TService, TEvent> AddRoutingKeyByExample(TEvent value);
    }

    
}