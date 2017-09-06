using System;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Astral.Configuration.Configs;
using Astral.Configuration.Settings;
using Astral.Contracts;
using Astral.Data;
using Astral.Deliveries;
using Astral.Payloads;
using Astral.Transport;
using FunEx;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Astral.Internals
{
    public class BusService<TService> : IBusService<TService>
        where TService : class

    {

        internal BusService(ServiceConfig<TService> config)
        {
            Config = config;
            Logger = config.LoggerFactory.CreateLogger<BusService<TService>>();
        }

        internal ServiceConfig<TService> Config { get; }
        
        public ILogger Logger { get; }

        /// <inheritdoc />
        public Task PublishAsync<TEvent>(Expression<Func<TService, IEvent<TEvent>>> selector, TEvent @event,
            EventPublishOptions options = null, CancellationToken token = default(CancellationToken))
        {
            var endpointConfig = Config.Endpoint(selector);
            var (transport, _, contentType) = endpointConfig.Transport;
            return PublishEventAsync(endpointConfig, @event, transport.PreparePublish<TEvent>, contentType, options, token);
        }

        public async Task<Guid> Deliver<TStore, TEvent>(TStore store, Expression<Func<TService, IEvent<TEvent>>> selector,
            TEvent @event, DeliveryOptions options = null)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>
        {
            var endpointConfig = Config.Endpoint(selector);
            var (transport, tag, contentType) = endpointConfig.Transport;
            var deliveryManager = Config.Provider.GetRequiredService<BoundDeliveryManager<TStore>>();
            var messageTtl = options?.MessageTtl ??
                             endpointConfig.TryGet<MessageTtl>().Map(p => p.Value).IfNone(Timeout.InfiniteTimeSpan);
            var afterCommit = (options?.AfterCommit).ToOption()
                .OrElse(() => endpointConfig.TryGet<AfterCommitDelivery>().Map(p => p.Value))
                .IfNone(DeliveryAfterCommit.Send(DeliveryOnSuccess.Delete));
            var toPayloadOptions = new ToPayloadOptions<byte[]>(contentType, endpointConfig.TypeEncoding.ToContract, endpointConfig.Serializer.Serialize);


            var poptions = new PublishOptions(messageTtl, ResponseTo.None, null);
            var key = endpointConfig.TryGet<IMessageKeyExtractor<TEvent>>().Map(p => p.ExtractKey(@event))
                .OrElse(() => (@event as IKeyProvider).ToOption().Map(p => p.Key))
                .Filter(_ => endpointConfig.TryGet<CleanSameKeyDelivery>().Map(p => p.Value).IfNone(true));

            var prepared = transport.PreparePublish<TEvent>(endpointConfig, poptions);
            return await deliveryManager.Prepare(store, @event, new DeliveryPoint(endpointConfig.SystemName, tag, endpointConfig.ServiceName, endpointConfig.EndpointName), 
                DeliveryOperation.Send, messageTtl, afterCommit, prepared, toPayloadOptions, key);
        }

        private Task PublishEventAsync<TEvent>(EndpointConfig config, TEvent @event, PreparePublish<TEvent> preparePublish, ContentType contentType, EventPublishOptions options = null,
            CancellationToken token = default(CancellationToken))
        {
            Task Publish()
            {

                var serialized = Payload
                    .ToPayload(@event,
                        new ToPayloadOptions<byte[]>(contentType, config.TypeEncoding.ToContract,
                            config.Serializer.Serialize))
                    .Unwrap();

                var poptions = new PublishOptions(
                    options?.EventTtl ?? config.TryGet<MessageTtl>().Map(p => p.Value).IfNone(Timeout.InfiniteTimeSpan),
                    ResponseTo.None, null);

                var prepared = preparePublish(config, poptions);

                return prepared(new Lazy<TEvent>(() => @event), serialized, token);
            }

            return Logger.LogActivity(Publish, "event {service} {endpoint}", config.ServiceType,
                config.PropertyInfo.Name);
        }
    }
}