using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Astral.Links;
using RabbitLink.Consumer;
using RabbitLink.Services.Descriptions;
using RabbitLink.Services.Exceptions;
using RabbitLink.Topology;

namespace RabbitLink.Services
{
    internal class EventConsumer<TService, TEvent> : ILinkEventConsumer<TService, TEvent>
        where TEvent : class
    {
        private string QueueName { get; }
        private ushort PrefetchCount { get; }
        private ServiceLink Link { get; }
        private EventDescription Description { get; }
        private bool AutoAck { get; }
        private ILinkConsumerErrorStrategy ErrorStrategy { get; }
        private bool? CancelOnHaFailover { get; }
        private bool Exclusive { get; }
        private bool ExchangePassive { get; }
        private bool QueuePassive { get; }
        private bool Bind { get; }
        private QueueParameters QueueParameters { get; }
        private List<string> RoutingKeys { get; }
        
        public EventConsumer(ServiceLink link, EventDescription description, string queueName = null,
            ushort prefetchCount = 1, bool autoAck = false, ILinkConsumerErrorStrategy errorStrategy = null,
            bool? cancelOnHaFailover = null, bool exclusive = false, bool exchangePassive = false,
            bool queuePassive = false, bool bind = true, QueueParameters queueParameters = null,
            List<string> routingKeys = null)
        {
            Link = link;
            Description = description;
            QueueName = queueName ?? $"{Link.ServiceName}.{Description.Service.Owner}.{Description.Service.Name}.{Description.Name}";
            PrefetchCount = prefetchCount;
            AutoAck = autoAck;
            ErrorStrategy = errorStrategy;
            CancelOnHaFailover = cancelOnHaFailover;
            Exclusive = exclusive;
            ExchangePassive = exchangePassive;
            QueuePassive = queuePassive;
            Bind = bind;
            QueueParameters = queueParameters;
            RoutingKeys = routingKeys;
        }

        private IDisposable Listen(Func<TEvent, CancellationToken, Task<Acknowledge>> listener) 
        {
            var builder = Link
                .Consumer
                .AutoAck(AutoAck);
            if (CancelOnHaFailover != null)
                builder = builder.CancelOnHaFailover(CancelOnHaFailover.Value);
            if (ErrorStrategy != null)
                builder = builder.ErrorStrategy(ErrorStrategy);
            builder = builder.Exclusive(Exclusive)
                .PrefetchCount(PrefetchCount);
            var parameters = QueueParameters ?? new QueueParameters();
            if(Description.Exchange.Type != LinkExchangeType.Fanout && Description.RoutingKey == null && (RoutingKeys == null || RoutingKeys.Count == 0))
                throw new InvalidConfigurationException($"No routing key for bind specified!");
            builder = builder.Queue(async cfg =>
            {
                var exchange = ExchangePassive
                    ? await cfg.ExchangeDeclarePassive(Description.Exchange.Name)
                    : await cfg.ExchangeDeclare(Description.Exchange.Name, Description.Exchange.Type,
                        Description.Exchange.Durable,
                        Description.Exchange.AutoDelete, Description.Exchange.Alternate, Description.Exchange.Delayed);
                var queue = QueuePassive
                    ? await cfg.QueueDeclarePassive(QueueName)
                    : await cfg.QueueDeclare(QueueName, parameters.Durable, parameters.Exclusive, parameters.AutoDelete,
                        parameters.MessageTtl, parameters.Expires, parameters.MaxPriority, parameters.MaxLength,
                        parameters.MaxLengthBytes, parameters.DeadLetterExchange, parameters.DeadLetterRoutingKey);
                if (Bind)
                {
                    if (Description.Exchange.Type == LinkExchangeType.Fanout)
                        await cfg.Bind(queue, exchange);
                    else if (Description.RoutingKey != null)
                        await cfg.Bind(queue, exchange, Description.RoutingKey);
                    else
                    {
                        foreach (var key in RoutingKeys)
                        {
                            await cfg.Bind(queue, exchange, key);
                        }
                    }
                }
                return queue;
            });
            builder = builder.Serializer(Link.PayloadManager.GetSerializer(Description.ContentType));

            builder = builder.Handler<TEvent>(async m =>
            {
                switch (await listener(m.Body, m.Cancellation))
                {
                    case Acknowledge.Ack:
                        return LinkConsumerAckStrategy.Ack;
                    case Acknowledge.Nack:
                        return LinkConsumerAckStrategy.Nack;
                    case Acknowledge.Requeue:
                        return LinkConsumerAckStrategy.Requeue;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
            
            return builder.Build();
        }

        IDisposable IEventConsumer<TEvent>.Listen(Func<TEvent, CancellationToken, Task<Acknowledge>> listener)
            => Listen(listener);
        
        ILinkEventConsumer<TService, TEvent> ILinkEventConsumer<TService, TEvent>.Queue(string queueName)
            => new EventConsumer<TService, TEvent>(Link, Description, queueName, PrefetchCount, AutoAck, ErrorStrategy, CancelOnHaFailover, Exclusive, ExchangePassive, QueuePassive, Bind, QueueParameters,
                RoutingKeys);

        ILinkEventConsumer<TService, TEvent> ILinkEventConsumer<TService, TEvent>.PrefetchCount(ushort value)
            => new EventConsumer<TService, TEvent>(Link, Description, QueueName, value, AutoAck, ErrorStrategy, CancelOnHaFailover, Exclusive, ExchangePassive, QueuePassive, Bind, QueueParameters,
                RoutingKeys);

        ILinkEventConsumer<TService, TEvent> ILinkEventConsumer<TService, TEvent>.AutoAck(bool value)
            => new EventConsumer<TService, TEvent>(Link, Description, QueueName, PrefetchCount, value, ErrorStrategy, CancelOnHaFailover, Exclusive, ExchangePassive, QueuePassive, Bind, QueueParameters,
                RoutingKeys);

        ILinkEventConsumer<TService, TEvent> ILinkEventConsumer<TService, TEvent>.ErrorStrategy(ILinkConsumerErrorStrategy value)
            => new EventConsumer<TService, TEvent>(Link, Description, QueueName, PrefetchCount, AutoAck, value, CancelOnHaFailover, Exclusive, ExchangePassive, QueuePassive, Bind, QueueParameters,
                RoutingKeys);

        ILinkEventConsumer<TService, TEvent> ILinkEventConsumer<TService, TEvent>.CancelOnHaFailover(bool value)
            => new EventConsumer<TService, TEvent>(Link, Description, QueueName, PrefetchCount, AutoAck, ErrorStrategy, value, Exclusive, ExchangePassive, QueuePassive, Bind, QueueParameters,
                RoutingKeys);

        ILinkEventConsumer<TService, TEvent> ILinkEventConsumer<TService, TEvent>.Exclusive(bool value)
            => new EventConsumer<TService, TEvent>(Link, Description, QueueName, PrefetchCount, AutoAck, ErrorStrategy, CancelOnHaFailover, value, ExchangePassive, QueuePassive, Bind, QueueParameters,
                RoutingKeys);

        ILinkEventConsumer<TService, TEvent> ILinkEventConsumer<TService, TEvent>.ExchangePassive(bool value)
            => new EventConsumer<TService, TEvent>(Link, Description, QueueName, PrefetchCount, AutoAck, ErrorStrategy, CancelOnHaFailover, Exclusive, value, QueuePassive, Bind, QueueParameters,
                RoutingKeys);

        ILinkEventConsumer<TService, TEvent> ILinkEventConsumer<TService, TEvent>.QueuePassive(bool value)
            => new EventConsumer<TService, TEvent>(Link, Description, QueueName, PrefetchCount, AutoAck, ErrorStrategy, CancelOnHaFailover, Exclusive, ExchangePassive, value, Bind, QueueParameters,
                RoutingKeys);

        ILinkEventConsumer<TService, TEvent> ILinkEventConsumer<TService, TEvent>.Bind(bool value)
            => new EventConsumer<TService, TEvent>(Link, Description, QueueName, PrefetchCount, AutoAck, ErrorStrategy, CancelOnHaFailover, Exclusive, ExchangePassive, QueuePassive, value, QueueParameters,
                RoutingKeys);

        ILinkEventConsumer<TService, TEvent> ILinkEventConsumer<TService, TEvent>.QueueParameters(QueueParameters parameters)
            => new EventConsumer<TService, TEvent>(Link, Description, QueueName, PrefetchCount, AutoAck, ErrorStrategy, CancelOnHaFailover, Exclusive, ExchangePassive, QueuePassive, Bind, parameters,
                RoutingKeys);

        ILinkEventConsumer<TService, TEvent> ILinkEventConsumer<TService, TEvent>.AddRoutingKey(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
            var keys = RoutingKeys ?? new List<string>();
            keys.Add(value);
            return new EventConsumer<TService, TEvent>(Link, Description, QueueName, PrefetchCount, AutoAck, ErrorStrategy,
                CancelOnHaFailover, Exclusive, ExchangePassive, QueuePassive, Bind, QueueParameters,
                keys);
        }

        ILinkEventConsumer<TService, TEvent> ILinkEventConsumer<TService, TEvent>.AddRoutingKeyByExample(TEvent value)
        {
            if(Description.RoutingKeyExtractor == null)
                throw new InvalidOperationException($"Routing key by example not supported!");
            return ((ILinkEventConsumer<TService, TEvent>) this).AddRoutingKey(Description.RoutingKeyExtractor(value));
        }
    }
}