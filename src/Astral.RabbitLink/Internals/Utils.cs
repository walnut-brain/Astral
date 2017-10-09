using System;
using System.Collections.Generic;
using System.Net.Mime;
using Astral.Markup.RabbitMq;
using Astral.RabbitLink.Descriptions;
using Astral.RabbitLink.Exceptions;
using Astral.Schema.RabbitMq;
using RabbitLink.Builders;
using RabbitLink.Consumer;
using RabbitLink.Producer;
using RabbitLink.Topology;

namespace Astral.RabbitLink.Internals
{
    internal static class Utils
    {
        public static LinkExchangeType ToLinkExchangeType(this ExchangeKind kind)
        {
            switch (kind)
            {
                case ExchangeKind.Fanout:
                    return LinkExchangeType.Fanout;
                case ExchangeKind.Direct:
                    return LinkExchangeType.Direct;
                case ExchangeKind.Topic:
                    return LinkExchangeType.Topic;
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }
        
        public static ILinkProducer CreateProducer(ServiceLink link, ExchangeSchema description,
            ContentType contentType, bool passive = false, bool confirmsMode = true, string named = null)
        {
            return
                link.GetOrAddProducer(named ?? description.Name, confirmsMode,() =>
                    link
                        .Producer
                        .ConfirmsMode(confirmsMode)
                        .Exchange(cfg =>
                            string.IsNullOrWhiteSpace(description.Name)
                                ? cfg.ExchangeDeclareDefault()
                                : passive
                                    ? cfg.ExchangeDeclarePassive(description.Name)
                                    : cfg.ExchangeDeclare(description.Name,
                                        description.Type.ToLinkExchangeType(), description.Durable, description.AutoDelete,
                                        description.Alternate, description.Delayed))
                        .Build());
        }

        public static ILinkConsumerBuilder CreateConsumerBuilder(ServiceLink link, ExchangeSchema exchange,
            bool exchangePassive, bool queuePassive,
            string queueName, bool autoAck, bool? cancelOnHaFailover, ILinkConsumerErrorStrategy errorStrategy,
            bool exclusive, ushort prefetchCount, QueueParameters queueParameters, ICollection<string> routingKeys,
            bool bind)
        {
            var builder = link
                .Consumer
                .AutoAck(autoAck);
            if (cancelOnHaFailover != null)
                builder = builder.CancelOnHaFailover(cancelOnHaFailover.Value);
            if (errorStrategy != null)
                builder = builder.ErrorStrategy(errorStrategy);
            builder = builder.Exclusive(exclusive)
                .PrefetchCount(prefetchCount);
            if(!string.IsNullOrWhiteSpace(exchange.Name) && 
               exchange.Type.ToLinkExchangeType() != LinkExchangeType.Fanout && 
               (routingKeys == null || routingKeys.Count == 0))
                throw new InvalidConfigurationException($"No routing key for bind specified!");
            builder = builder.Queue(async cfg =>
            {
                var exch =
                    string.IsNullOrWhiteSpace(exchange.Name)
                        ? await cfg.ExchangeDeclareDefault()
                        :
                        exchangePassive  
                            ? await cfg.ExchangeDeclarePassive(exchange.Name)
                            : await cfg.ExchangeDeclare(exchange.Name, exchange.Type.ToLinkExchangeType(),
                                exchange.Durable,
                                exchange.AutoDelete, exchange.Alternate, exchange.Delayed);
                var queue = queuePassive
                    ? await cfg.QueueDeclarePassive(queueName)
                    : await cfg.QueueDeclare(queueName, queueParameters.Durable(), queueParameters.Exclusive(), queueParameters.AutoDelete(),
                        queueParameters.MessageTtl(), queueParameters.Expires(), queueParameters.MaxPriority(), queueParameters.MaxLength(),
                        queueParameters.MaxLengthBytes(), queueParameters.DeadLetterExchange(), queueParameters.DeadLetterRoutingKey());
                if (!string.IsNullOrWhiteSpace(exchange.Name) && bind)
                {
                    if (exchange.Type.ToLinkExchangeType() == LinkExchangeType.Fanout)
                        await cfg.Bind(queue, exch);
                    else
                    {
                        foreach (var key in routingKeys)
                        {
                            await cfg.Bind(queue, exch, key);
                        }
                    }
                }
                return queue;
            });
            return builder;
        }
    }
}