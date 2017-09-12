using System;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Astral.Configuration;
using Astral.Configuration.Builders;
using Astral.Configuration.Settings;
using Astral.Contracts;
using Astral.Data;
using Astral.Deliveries;
using Astral.Payloads;
using Astral.Payloads.DataContracts;
using Astral.Payloads.Serialization;
using Astral.Specifications;
using Astral.Transport;
using FunEx;
using FunEx.Monads;
using Lawium;
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



        #region Event

        /// <inheritdoc />
        public Task PublishAsync<TEvent>(Expression<Func<TService, IEvent<TEvent>>> selector, TEvent @event,
            CancellationToken token = default(CancellationToken))
        {
            var config = Config.Endpoint(selector);
            var publishOptions = new PublishOptions(
                config.TryGetService<MessageTtlSetting>().Map(p => p.Value)
                    .OrElse(() => config.TryGetService<MessageTtlFactorySetting<TEvent>>().Map(p => p.Value(@event)))
                    .IfNone(Timeout.InfiniteTimeSpan),
                ResponseTo.None, null);
            return PublishMessageAsync(config, @event, publishOptions, token);
        }


        public Task<Guid> Deliver<TStore, TEvent>(TStore store,
            Expression<Func<TService, IEvent<TEvent>>> selector,
            TEvent @event, DeliveryOnSuccess? onSuccess = null)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>
        {
            var endpoint = Config.Endpoint(selector);
            
            return Deliverer(store, endpoint, @event, DeliveryReply.NoReply, 
                onSuccess ?? endpoint.TryGetService<DeliveryOnSuccessSetting>().Map(p => p.Value).IfNone(DeliveryOnSuccess.Delete));
        }

        public Task<Guid> Enqueue<TStore, TEvent>(TStore store,
            Expression<Func<TService, IEvent<TEvent>>> selector,
            TEvent @event)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>
        {
            var endpoint = Config.Endpoint(selector);
            return Deliverer(store, endpoint, @event, DeliveryReply.NoReply, Option.None);
        }


        /// <inheritdoc />
        public IDisposable Listen<TEvent>(Expression<Func<TService, IEvent<TEvent>>> selector,
            IListener<TEvent, EventContext> eventListener, SubscribeChannel? channel = null, Action<ChannelBuilder> configure = null) 
            => Listen(Config.Endpoint(selector).Channel(channel ?? SubscribeChannel.System, false, configure ?? (_ => { })), 
                eventListener, p => throw new NotImplementedException());

        #endregion

        #region ICall<>

        public async Task<Guid> Send<TCommand>(Expression<Func<TService, ICall<TCommand>>> selector, TCommand command,
            ResponseTo responseTo = null, CancellationToken cancellation = default(CancellationToken))
        {
            var endpoint = Config.Endpoint(selector);
            responseTo = responseTo ??
                         endpoint.TryGetService<ResponseToSetting>().Map(p => p.Value).IfNone(ResponseTo.System);
            var ttl = endpoint.TryGetService<MessageTtlSetting>().Map(p => p.Value)
                .OrElse(() => endpoint.TryGetService<MessageTtlFactorySetting<TCommand>>().Map(p => p.Value(command)))
                .IfNone(Timeout.InfiniteTimeSpan);
            var correlationId = Guid.NewGuid();
            var option = new PublishOptions(ttl, responseTo, correlationId);
            var payload = endpoint.ToPayload(command).Unwrap();
            var sender = endpoint.Transport.PreparePublish<TCommand>(endpoint, option);
            await sender(new Lazy<TCommand>(() => command), payload, cancellation);
            return correlationId;
        }
        
        public async Task Response<TCommand>(Expression<Func<TService, ICall<TCommand>>> selector,
            ReplayToInfo replayTo, CancellationToken cancellation = default(CancellationToken))
        {
            if (replayTo == null) throw new ArgumentNullException(nameof(replayTo));
            var endpoint = Config.Endpoint(selector);
            var option = new PublishOptions(Timeout.InfiniteTimeSpan, ResponseTo.Answer(replayTo.ReplayTo, replayTo.ReplayOn), null);
            var payload = endpoint.ToPayload(default(ValueTuple)).Unwrap();
            var sender = endpoint.Transport.PreparePublish<ValueTuple>(endpoint, option);
            await sender(new Lazy<ValueTuple>(() => default(ValueTuple)), payload, cancellation);
        }
        
        public Task<Guid> Deliver<TStore, TCommand>(TStore store, Expression<Func<TService, ICall<TCommand>>> selector,
            TCommand command, DeliveryOnSuccess? onSuccess = null, DeliveryReplyTo? replyTo = null)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>
        {
            var endpoint = Config.Endpoint(selector);
            return Deliverer(store, endpoint, command,
                DeliveryReply.WithReply(replyTo ?? endpoint.TryGetService<DeliveryReplayToSetting>().Map(p => p.Value)
                                            .IfNone(DeliveryReplyTo.System)),
                onSuccess ?? endpoint.TryGetService<DeliveryOnSuccessSetting>().Map(p => p.Value)
                    .IfNone(DeliveryOnSuccess.Archive));
        }
        
        

        public Task<Guid> Enqueue<TStore, TCommand>(TStore store,
            Expression<Func<TService, ICall<TCommand>>> selector, TCommand command, DeliveryReplyTo? replyTo = null)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>
        {
            var endpoint = Config.Endpoint(selector);
            return Deliverer(store, endpoint, command,
                DeliveryReply.WithReply(replyTo ?? endpoint.TryGetService<DeliveryReplayToSetting>().Map(p => p.Value)
                                            .IfNone(DeliveryReplyTo.System)), Option.None);
        }

        public Task<Guid> DeliverReply<TStore, TCommand>(TStore store,
            Expression<Func<TService, ICall<TCommand>>> selector,
            ReplayToInfo replayTo)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>
        {
            var endpoint = Config.Endpoint(selector);
            return Deliverer(store, endpoint, default(ValueTuple),
                DeliveryReply.IsReply(replayTo.ReplayTo, replayTo.ReplayOn),
                DeliveryOnSuccess.Delete);
        }

        

        #endregion
        
        #region ICall<,>
        
        public Task<Guid> Deliver<TStore, TRequest, TResponse>(TStore store, Expression<Func<TService, ICall<TRequest, TResponse>>> selector,
            TRequest command, DeliveryOnSuccess? onSuccess = null, DeliveryReplyTo? replyTo = null)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>
        {
            var endpoint = Config.Endpoint(selector);
            return Deliverer(store, endpoint, command,
                DeliveryReply.WithReply(replyTo ?? endpoint.TryGetService<DeliveryReplayToSetting>().Map(p => p.Value)
                                            .IfNone(DeliveryReplyTo.System)),
                onSuccess ?? endpoint.TryGetService<DeliveryOnSuccessSetting>().Map(p => p.Value)
                    .IfNone(DeliveryOnSuccess.Archive));
        }

        public Task<Guid> Enqueue<TStore, TRequest, TResponse>(TStore store,
            Expression<Func<TService, ICall<TRequest, TResponse>>> selector, TRequest command, DeliveryReplyTo? replyTo = null)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>
        {
            var endpoint = Config.Endpoint(selector);
            return Deliverer(store, endpoint, command,
                DeliveryReply.WithReply(replyTo ?? endpoint.TryGetService<DeliveryReplayToSetting>().Map(p => p.Value)
                                            .IfNone(DeliveryReplyTo.System)), Option.None);
        }

        public Task<Guid> DeliverReply<TStore, TRequest, TReplay>(TStore store,
            Expression<Func<TService, ICall<TRequest, TReplay>>> selector, TReplay replay,
            ReplayToInfo replayTo)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>
        {
            var endpoint = Config.Endpoint(selector);
            return Deliverer(store, endpoint, replay,
                DeliveryReply.IsReply(replayTo.ReplayTo, replayTo.ReplayOn),
                DeliveryOnSuccess.Delete);
        }
        
        #endregion
        
        private async Task<Guid> Deliverer<TStore, TMessage>(TStore store, EndpointConfig endpoint, TMessage message,
            DeliveryReply reply, Option<DeliveryOnSuccess> onSuccess)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>
        {
            var deliveryId = Guid.NewGuid();
            var deliveryManager = Config.GetRequiredService<BoundDeliveryManager<TStore>>();
            var messageTtl = endpoint.TryGetService<MessageTtlSetting>().Map(p => p.Value)
                .OrElse(() => endpoint.TryGetService<MessageTtlFactorySetting<TMessage>>().Map(p => p.Value(message)))
                .IfNone(Timeout.InfiniteTimeSpan);
            var sender = endpoint.Transport.PreparePublish<TMessage>(endpoint,
                new PublishOptions(messageTtl, reply.Match(() => ResponseTo.None, rt => rt.ResponseTo, 
                    ResponseTo.Answer), deliveryId));
            
            await deliveryManager.Prepare(store, endpoint, deliveryId, message, reply, sender, onSuccess);
            return deliveryId;
        }
        
        
        
        private Task PublishMessageAsync<TEvent>(EndpointConfig config, TEvent @event, PublishOptions options, CancellationToken token)
        {
            Task Publish()
            {
                var serialized = config.ToPayload(@event).Unwrap();
                var prepared = config.Transport.PreparePublish<TEvent>(config, options);
                return prepared(new Lazy<TEvent>(() => @event), serialized, token);
            }

            return Logger.LogActivity(Publish, "event {service} {endpoint}", config.ServiceType,
                config.PropertyInfo.Name);
        }
        
       
        private IDisposable Listen<TEvent, TContext>(ChannelConfig config,
            IListener<TEvent, TContext> eventListener,
            Func<MessageContext, TContext> contextConverter)
        {
            return Logger.LogActivity(Listen, "listen {service} {endpoint}", config.Endpoint.ServiceType,
                config.Endpoint.PropertyInfo.Name);

            IDisposable Listen()
            {
                var exceptionPolicy = config.Endpoint.AsTry<ExceptionToAcknowledgeSetting>().Map(p => p.Value).RecoverTo(p => CommonLaws.DefaultExceptionPolicy(p));


                return config.Endpoint.Transport.Subscribe(config, (msg, ctx, token) => Listener(msg, ctx, token, exceptionPolicy));
            }

            async Task<Acknowledge> Listener(
                Payload<byte[]> msg, MessageContext ctx, CancellationToken token, Func<Exception, Acknowledge> exceptionPolicy)
            {
                async Task<Acknowledge> Receive()
                {
                    var obj = config.Endpoint.FromPayload(msg).As<TEvent>().Unwrap();
                    
                    await eventListener.Handle(obj, contextConverter(ctx), token);
                    return Acknowledge.Ack;
                }

                return await Receive()
                    .LogResult(Logger, "recive event {service} {endpoint}", config.Endpoint.ServiceType, config.Endpoint.PropertyInfo)
                    .CorrectError(exceptionPolicy);
            }
        }

        

        
    }

    public class ResponseContext
    {
    }

    public class SendCallOptions
    {
        public TimeSpan? MessageTtl { get; }
        public ResponseTo ResponseTo { get; }
    }
}