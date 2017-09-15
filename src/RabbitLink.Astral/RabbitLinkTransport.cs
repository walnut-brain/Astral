using System;
using System.Threading;
using System.Threading.Tasks;
using Astral;
using Astral.Specifications;
using Astral.Transport;
using Astral.Utils;
using Lawium;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitLink.Astral.Settings;
using RabbitLink.Messaging;
using RabbitLink.Producer;

namespace RabbitLink.Astral
{
    public class RabbitLinkTransport : ITransport, IDisposable
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILink _link;
        private readonly BlockedDisposableDictionary<(string, bool), ILinkProducer> _producers = new BlockedDisposableDictionary<(string, bool), ILinkProducer>();

        public RabbitLinkTransport(string url, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _link = 
                LinkBuilder
                    .Configure
                    .Uri(url)
                    .LoggerFactory(new LinkLoggerFactory(loggerFactory))
                    .Build();
        }

        public PayloadSender<TMessage> PreparePublish<TMessage>(EndpointConfig config, bool isReply, ChannelKind responseTo)
        {
            PayloadSender<TMessage> Sender(ExchangeConfig exchangeConfig,
                PublishMessageProperties<TMessage> properties)
                => (msg, payload, correlationId, cancellation) =>
                {
                    var producer = _producers.GetOrAdd((exchangeConfig.Name, exchangeConfig.ConfirmsMode), _ =>
                    {
                        return _link
                            .Producer
                            .Exchange(tcfg =>
                                exchangeConfig.AsPassive
                                    ? tcfg.ExchangeDeclarePassive(exchangeConfig.Name)
                                    : tcfg.ExchangeDeclare(exchangeConfig.Name, exchangeConfig.Type,
                                        exchangeConfig.Durable))
                            .ConfirmsMode(exchangeConfig.ConfirmsMode)
                            .Build();
                    });
                    var message = new LinkPublishMessage(payload.Data);
                    message.Properties.AppId = properties.Sender;
                    message.Properties.ContentType = payload.ContentType.ToString();
                    message.Properties.Type = payload.TypeCode;
                    if (!string.IsNullOrWhiteSpace(correlationId))
                        message.Properties.CorrelationId = correlationId;
                    if (!string.IsNullOrWhiteSpace(properties.ReplyTo))
                        message.Properties.ReplyTo = properties.ReplyTo;
                    message.Properties.DeliveryMode = properties.DeliveryMode;
                    var ttl = properties.MessageTtl(msg);
                    if (ttl != Timeout.InfiniteTimeSpan)
                        message.Properties.Expiration = ttl;
                    var routingKey = properties.RoutingKey(msg);
                    if (!string.IsNullOrWhiteSpace(routingKey))
                        message.PublishProperties.RoutingKey = routingKey;
                    return producer.PublishAsync(message, cancellation);
                };
            var exConfig = new ExchangeConfig(
                config.GetRequiredService<ExchangeNameSetting>().Value,
                config.GetRequiredService<ExchangeTypeSetting>().Value,
                config.GetRequiredService<PassiveExchangeDeclareSetting>().Value,
                config.GetRequiredService<DurableExchangeSetting>().Value,
                config.GetRequiredService<ConfirmsModeSetting>().Value);
            /*var pmp = new PublishMessageProperties<TMessage>(config.SystemName, 
                );*/
            throw new NotImplementedException();
        }

        public (string, Subscribable) GetChannel(ChannelConfig config)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}