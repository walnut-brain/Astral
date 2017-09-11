using System;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Astral.Configuration;
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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Astral.Internals
{
    public class BusService<TService> : IBusService<TService>
        where TService : class

    {

        internal BusService(ServiceSpecification<TService> specification)
        {
            Specification = specification;
            Logger = specification.LoggerFactory.CreateLogger<BusService<TService>>();
        }

        internal ServiceSpecification<TService> Specification { get; }

        public ILogger Logger { get; }



        #region Event

        /// <inheritdoc />
        public Task PublishAsync<TEvent>(Expression<Func<TService, IEvent<TEvent>>> selector, TEvent @event,
            EventPublishOptions options = null, CancellationToken token = default(CancellationToken))
        {
            var config = Specification.Endpoint(selector);
            var publishOptions = new PublishOptions(
                options?.EventTtl ?? config.TryGetService<MessageTtl>().Map(p => p.Value).IfNone(Timeout.InfiniteTimeSpan),
                ResponseTo.None, null);
            return PublishMessageAsync(config, @event, publishOptions, token);
        }


        /// <inheritdoc />
        public Task<Guid> Deliver<TStore, TEvent>(TStore store,
            Expression<Func<TService, IEvent<TEvent>>> selector,
            TEvent @event, TimeSpan? messageTtl = null, DeliveryOnSuccess onSuccess = null)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>
            => Deliver(store, selector, @event, messageTtl, onSuccess.ToOption().Map(DeliveryAfterCommit.Send));

        /// <inheritdoc />
        public Task<Guid> SaveDelivery<TStore, TEvent>(TStore store,
            Expression<Func<TService, IEvent<TEvent>>> selector,
            TEvent @event, TimeSpan? messageTtl = null)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>
            => Deliver(store, selector, @event, messageTtl, DeliveryAfterCommit.NoOp);
        
        private async Task<Guid> Deliver<TStore, TEvent>(TStore store,
            Expression<Func<TService, IEvent<TEvent>>> selector,
            TEvent @event, TimeSpan? messageTtl, Option<DeliveryAfterCommit> afterCommit = default(Option<DeliveryAfterCommit>))
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>
        {
            var endpoint = Specification.Endpoint(selector);
            var deliveryCreateParams = new DeliveryCreateParams<TEvent>(
                Guid.NewGuid(), null, endpoint.SystemName, endpoint.ServiceName, endpoint.EndpointName, 
                endpoint.TryGetService<MessageKeyExtractor<TEvent>>()
                    .Map(p => p.Value)
                    .Map(p => p(@event))
                    .OrElse(() => (@event as IKeyProvider).ToOption().Map(p => p.Key))
                    .Filter(_ => endpoint.TryGetService<CleanSameKeyDelivery>().Map(p => p.Value).IfNone(true)).IfNoneDefault(),
                null, null, @event, false);
            var encoder = endpoint.Transport.PayloadEncode;
            var msgTtl = messageTtl ??
                         endpoint.TryGetService<MessageTtl>().Map(p => p.Value).IfNone(Timeout.InfiniteTimeSpan);
            var sender = endpoint.Transport.Provider.PreparePublish<TEvent>(endpoint,
                new PublishOptions(msgTtl, ResponseTo.None, null));
            return await DeliverMessage(store, deliveryCreateParams, sender, msgTtl,
                 afterCommit
                     .OrElse(() => endpoint.TryGetService<RequestDeliveryPolicy>().Map(p => p.Value))
                     .IfNone(() => DeliveryAfterCommit.Send(DeliveryOnSuccess.Delete)),
                encoder);
        }

        /// <inheritdoc />
        public IDisposable Listen<TEvent>(Expression<Func<TService, IEvent<TEvent>>> selector,
            IEventListener<TEvent> eventListener, EventListenOptions options = null) 
            => ListenEvent(Logger, Specification.Endpoint(selector), eventListener, options);

        #endregion


        public Task<Guid> Deliver<TStore, TCommand>(TStore store, Expression<Func<TService, ICall<TCommand>>> selector,
            TCommand command,
            string target = null, TimeSpan? messageTtl = null, DeliveryOnSuccess onSuccess = null,
            DeliveryReplyTo replyTo = null)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>
            => Deliver(store, selector, command, target, messageTtl, onSuccess.ToOption().Map(DeliveryAfterCommit.Send), replyTo);

        public Task<Guid> SaveDelivery<TStore, TCommand>(TStore store,
            Expression<Func<TService, ICall<TCommand>>> selector, TCommand command,
            string target = null, TimeSpan? messageTtl = null, DeliveryReplyTo replyTo = null)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>
            => Deliver(store, selector, command, target, messageTtl, DeliveryAfterCommit.NoOp, replyTo);
        
        private Task<Guid> Deliver<TStore, TCommand>(TStore store, Expression<Func<TService, ICall<TCommand>>> selector, TCommand command,
            string target, TimeSpan? messageTtl, Option<DeliveryAfterCommit> afterCommit, DeliveryReplyTo replyTo)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>
        {
            var endpoint = Specification.Endpoint(selector);
            var deliveryCreateParams = new DeliveryCreateParams<TCommand>(Guid.NewGuid(),
                target ?? endpoint.GetRequiredService<ServiceOwner>().Value,
                endpoint.SystemName, endpoint.ServiceName, endpoint.EndpointName,
                endpoint.TryGetService<MessageKeyExtractor<TCommand>>()
                    .Map(p => p.Value)
                    .Map(p => p(command))
                    .OrElse(() => (command as IKeyProvider).ToOption().Map(p => p.Key))
                    .Filter(_ => endpoint.TryGetService<CleanSameKeyDelivery>().Map(p => p.Value).IfNone(true))
                    .IfNoneDefault(),
                replyTo ?? DeliveryReplyTo.System, null, command, false);
            var encoder = endpoint.Transport.PayloadEncode;
            var msgTtl = messageTtl ??
                         endpoint.TryGetService<MessageTtl>().Map(p => p.Value).IfNone(Timeout.InfiniteTimeSpan);
            var sender = endpoint.Transport.Provider.PreparePublish<TCommand>(endpoint,
                new PublishOptions(msgTtl, (replyTo ?? DeliveryReplyTo.System).ResponseTo, null));
            return DeliverMessage(store, deliveryCreateParams, sender, msgTtl, afterCommit
                .OrElse(() => endpoint.TryGetService<RequestDeliveryPolicy>().Map(p => p.Value))
                .IfNone(() => DeliveryAfterCommit.Send(DeliveryOnSuccess.Delete)), encoder); 
        }
        
        
        
        private Task PublishMessageAsync<TEvent>(EndpointSpecification specification, TEvent @event, PublishOptions options, CancellationToken token)
        {
            Task Publish()
            {
                var serialized = specification.Transport.ToPayload(@event).Unwrap();
                var prepared = specification.Transport.Provider.PreparePublish<TEvent>(specification, options);
                return prepared(new Lazy<TEvent>(() => @event), serialized, token);
            }

            return Logger.LogActivity(Publish, "event {service} {endpoint}", specification.ServiceType,
                specification.PropertyInfo.Name);
        }
        
        private async Task<Guid> DeliverMessage<TStore, TEvent>(TStore store,
                DeliveryCreateParams<TEvent> parameters, 
                PayloadSender<TEvent> sender, TimeSpan messageTtl, DeliveryAfterCommit afterCommit,
                PayloadEncode<byte[]> payloadEncode)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>
        {
            var deliveryManager = Specification.GetRequiredService<BoundDeliveryManager<TStore>>();
            
            await deliveryManager.Prepare(store, parameters, sender, messageTtl, afterCommit, payloadEncode); 
                
            return parameters.DeliveryId;
        }


        


        private IDisposable ListenEvent<TEvent>(ILogger logger, EndpointSpecification specification,
            IEventListener<TEvent> eventListener,
            EventListenOptions options)
        {
            return logger.LogActivity(Listen, "listen {service} {endpoint}", specification.ServiceType,
                specification.PropertyInfo.Name);

            IDisposable Listen()
            {
                var exceptionPolicy = specification.AsTry<RecieveExceptionPolicy>().Map(p => p.Value).RecoverTo(p => CommonLaws.DefaultExceptionPolicy(p));


                return specification.Transport.Provider.Subscribe(specification, (msg, ctx, token) => Listener(msg, ctx, token, exceptionPolicy), options);
            }

            async Task<Acknowledge> Listener(
                Payload<byte[]> msg, EventContext ctx, CancellationToken token, Func<Exception, Acknowledge> exceptionPolicy)
            {
                async Task<Acknowledge> Receive()
                {
                    var obj = specification.Transport.FromPayload(msg).As<TEvent>().Unwrap();

                    await eventListener.Handle(obj, ctx, token);
                    return Acknowledge.Ack;
                }

                return await Receive()
                    .LogResult(logger, "recive event {service} {endpoint}", specification.ServiceType, specification.PropertyInfo)
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