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
        public async Task<Guid> Deliver<TStore, TEvent>(TStore store,
            Expression<Func<TService, IEvent<TEvent>>> selector,
            TEvent @event, DeliveryOptions options = null)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>
        {
            var endpoint = Specification.Endpoint(selector);
            return await DeliverMessage(store, @event, endpoint, options, DeliveryOperation.Send);
        }

        /// <inheritdoc />
        public IDisposable Listen<TEvent>(Expression<Func<TService, IEvent<TEvent>>> selector,
            IEventListener<TEvent> eventListener, EventListenOptions options = null) 
            => ListenEvent(Logger, Specification.Endpoint(selector), eventListener, options);

        public Task<Guid> Deliver<TStore, TCommand>(TStore store, Expression<Func<TService, ICall<TCommand>>> selector, TCommand command, DeliveryOptions options = null)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>
        {
            var config = Specification.Endpoint(selector);

            return DeliverMessage(store, command, config, options,
                DeliveryOperation.SendWithReply(options?.ReplyTo ?? DeliveryReplyTo.System));
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
            TEvent @event, EndpointSpecification specification, DeliveryOptions options,  DeliveryOperation operation)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>
        {
            var deliveryManager = Specification.GetRequiredService<BoundDeliveryManager<TStore>>();
            var messageTtl = options?.MessageTtl ??
                             specification.TryGetService<MessageTtl>().Map(p => p.Value).IfNone(Timeout.InfiniteTimeSpan);
            var afterCommit = options?.AfterCommit ??
                              specification
                                  .TryGetService<AfterCommitDelivery>()
                                  .Map(p => p.Value)
                                  .IfNone(DeliveryAfterCommit.Send(DeliveryOnSuccess.Delete));

            var deliveryId = Guid.NewGuid();
            
            var poptions = new PublishOptions(messageTtl, 
                operation.ResponseTo, deliveryId);
            var key = specification.TryGetService<MessageKeyExtractor<TEvent>>()
                .Map(p => p.Value)
                .Map(p => p(@event))
                .OrElse(() => (@event as IKeyProvider).ToOption().Map(p => p.Key))
                .Filter(_ => specification.TryGetService<CleanSameKeyDelivery>().Map(p => p.Value).IfNone(true));

            var prepared = specification.Transport.Provider.PreparePublish<TEvent>(specification, poptions);
            await deliveryManager.Prepare(store, @event, deliveryId,
                specification.Transport,new DeliveryParams(operation, afterCommit, messageTtl),
                prepared, key);
            return deliveryId;
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