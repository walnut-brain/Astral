using System;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Astral.Configuration;
using Astral.Configuration.Configs;
using Astral.Configuration.Settings;
using Astral.Contracts;
using Astral.Data;
using Astral.Deliveries;
using Astral.Payloads;
using Astral.Payloads.DataContracts;
using Astral.Payloads.Serialization;
using Astral.Transport;
using FunEx;
using FunEx.Monads;
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
            var config = Config.Endpoint(selector);
            var publishOptions = new PublishOptions(
                    options?.EventTtl ?? config.TryGet<MessageTtl>().Map(p => p.Value).IfNone(Timeout.InfiniteTimeSpan),
                    ResponseTo.None, null);
            return PublishMessageAsync(config, @event, publishOptions, token);
        }

        /// <inheritdoc />
        public async Task<Guid> Deliver<TStore, TEvent>(TStore store,
            Expression<Func<TService, IEvent<TEvent>>> selector,
            TEvent @event, DeliveryOptions options = null)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>
        {
            var endpoint = Config.Endpoint(selector);
            return await DeliverMessage(store, @event, endpoint, options, DeliveryOperation.Send);
        }

        /// <inheritdoc />
        public IDisposable Listen<TEvent>(Expression<Func<TService, IEvent<TEvent>>> selector,
            IEventListener<TEvent> eventListener, EventListenOptions options = null) 
            => ListenEvent(Logger, Config.Endpoint(selector), eventListener, options);

        public Task<Guid> Deliver<TStore, TCommand>(TStore store, Expression<Func<TService, ICall<TCommand>>> selector, TCommand command, DeliveryOptions options = null)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>
        {
            var config = Config.Endpoint(selector);

            return DeliverMessage(store, command, config, options,
                DeliveryOperation.SendWithReply(options?.ReplyTo ?? DeliveryReplyTo.System));
        }
        
        
        
        private Task PublishMessageAsync<TEvent>(EndpointConfig config, TEvent @event, PublishOptions options, CancellationToken token)
        {
            Task Publish()
            {
                var serialized = config.Transport.ToPayload(@event).Unwrap();
                var prepared = config.Transport.Transport.PreparePublish<TEvent>(config, options);
                return prepared(new Lazy<TEvent>(() => @event), serialized, token);
            }

            return Logger.LogActivity(Publish, "event {service} {endpoint}", config.ServiceType,
                config.PropertyInfo.Name);
        }
        
        private async Task<Guid> DeliverMessage<TStore, TEvent>(TStore store,
            TEvent @event, EndpointConfig config, DeliveryOptions options,  DeliveryOperation operation)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>
        {
            var deliveryManager = Config.GetRequiredService<BoundDeliveryManager<TStore>>();
            var messageTtl = options?.MessageTtl ??
                             config.TryGet<MessageTtl>().Map(p => p.Value).IfNone(Timeout.InfiniteTimeSpan);
            var afterCommit = options?.AfterCommit ??
                              config
                                  .TryGet<AfterCommitDelivery>()
                                  .Map(p => p.Value)
                                  .IfNone(DeliveryAfterCommit.Send(DeliveryOnSuccess.Delete));

            var deliveryId = Guid.NewGuid();
            
            var poptions = new PublishOptions(messageTtl, 
                operation.ResponseTo, deliveryId);
            var key = config.TryGet<MessageKeyExtractor<TEvent>>()
                .Map(p => p.Value)
                .Map(p => p(@event))
                .OrElse(() => (@event as IKeyProvider).ToOption().Map(p => p.Key))
                .Filter(_ => config.TryGet<CleanSameKeyDelivery>().Map(p => p.Value).IfNone(true));

            var prepared = config.Transport.Transport.PreparePublish<TEvent>(config, poptions);
            await deliveryManager.Prepare(store, @event, deliveryId,
                new DeliveryPoint(config.SystemName, config.Transport.TransportTag, config.ServiceName,
                    config.EndpointName),
                operation, messageTtl, afterCommit, prepared, config.Transport.ToPayloadOptions, key);
            return deliveryId;
        }


        


        private IDisposable ListenEvent<TEvent>(ILogger logger, EndpointConfig config,
            IEventListener<TEvent> eventListener,
            EventListenOptions options)
        {
            return logger.LogActivity(Listen, "listen {service} {endpoint}", config.ServiceType,
                config.PropertyInfo.Name);

            IDisposable Listen()
            {
                var exceptionPolicy = config.AsTry<RecieveExceptionPolicy>().Map(p => p.Value).RecoverTo(p => CommonLaws.DefaultExceptionPolicy(p));


                return config.Transport.Transport.Subscribe(config, (msg, ctx, token) => Listener(msg, ctx, token, exceptionPolicy), options);
            }

            async Task<Acknowledge> Listener(
                Payload<byte[]> msg, EventContext ctx, CancellationToken token, Func<Exception, Acknowledge> exceptionPolicy)
            {
                async Task<Acknowledge> Receive()
                {
                    var obj = config.Transport.FromPayload(msg).As<TEvent>().Unwrap();

                    await eventListener.Handle(obj, ctx, token);
                    return Acknowledge.Ack;
                }

                return await Receive()
                    .LogResult(logger, "recive event {service} {endpoint}", config.ServiceType, config.PropertyInfo)
                    .CorrectError(exceptionPolicy);
            }
        }

        

        
    }

    public class SendCallOptions
    {
        public TimeSpan? MessageTtl { get; }
        public ResponseTo ResponseTo { get; }
    }
}