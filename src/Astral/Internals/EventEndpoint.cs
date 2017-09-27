using System;
using System.Threading;
using System.Threading.Tasks;
using Astral.Configuration;
using Astral.Configuration.Builders;
using Astral.Configuration.Settings;
using Astral.Data;
using Astral.Deliveries;
using Astral.Payloads;
using Astral.Specifications;
using FunEx.Monads;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Astral.Internals
{
    internal class EndpointBase
    {
        protected EndpointConfig Config { get; }
        protected ILogger Logger { get; } 

        public EndpointBase(EndpointConfig config)
        {
            Config = config;
            Logger = config.GetService<ILoggerFactory>().CreateLogger(GetType());
        }

        protected Task PublishMessageAsync<TEvent>(TEvent @event, ChannelKind channel, CancellationToken token)
        {
            Task Publish()
            {
                var serialized = Config.ToPayload(@event).Unwrap();
                var prepared = Config.Transport.PreparePublish<TEvent>(Config, false, channel);
                return prepared(new Lazy<TEvent>(() => @event), serialized, null, token);
            }

            return Logger.LogActivity(Publish, "event {service} {endpoint}", Config.ServiceType,
                Config.PropertyInfo.Name);
        }

        protected async Task<Guid> Deliverer<TStore, TMessage>(IUnitOfWork<TStore> uow, TMessage message,
            bool isReply, DeliveryReply reply, Option<DeliveryOnSuccess> onSuccess)
        {
            var deliveryId = Guid.NewGuid();
            var deliveryManager = Config.GetRequiredService<BoundDeliveryManager<TStore>>();
            var channel = reply.Match(() => (ChannelKind.RespondableChannel) ChannelKind.None, rt => rt, rt => rt);
            var sender = Config.Transport.PreparePublish<TMessage>(Config, isReply, channel);
            await deliveryManager.Prepare(uow, Config, deliveryId, message, reply, sender, onSuccess);
            return deliveryId;
        }

        protected IDisposable Listen<TEvent, TContext>(ChannelConfig channel,
            Func<TEvent, TContext, CancellationToken, Task> eventListener,
            Func<MessageContext, TContext> contextConverter)
            => Listen(channel, eventListener, p => Config.FromPayload(p).As<TEvent>().Unwrap(), contextConverter);

        protected IDisposable Listen<TEvent, TContext>(ChannelConfig channel,
            Func<TEvent, TContext, CancellationToken, Task> eventListener, Func<Payload<byte[]>, TEvent> converter, 
            Func<MessageContext, TContext> contextConverter)
        {
            return Logger.LogActivity(Listen, "listen {service} {endpoint}", Config.ServiceType,
                Config.PropertyInfo.Name);

            IDisposable Listen()
            {
                var exceptionPolicy = Config.AsTry<ExceptionToAcknowledge>().RecoverTo(p => CommonLaws.DefaultExceptionPolicy(p));


                var (_, subscribable) = Config.Transport.GetChannel(channel);
                return subscribable((msg, ctx, token) => Listener(msg, ctx, token, ex => exceptionPolicy(ex)));
            }

            async Task<Acknowledge> Listener(
                Payload<byte[]> msg, MessageContext ctx, CancellationToken token, Func<Exception, Acknowledge> exceptionPolicy)
            {
                async Task<Acknowledge> Receive()
                {
                    var obj = converter(msg); 
                    await eventListener(obj, contextConverter(ctx), token);
                    return Acknowledge.Ack;
                }

                return await Receive()
                    .LogResult(Logger, "recive event {service} {endpoint}", channel.Endpoint.ServiceType, channel.Endpoint.PropertyInfo)
                    .CorrectError(exceptionPolicy);
            }
        }
    }
    
    internal class EventEndpoint<TEvent> : EndpointBase, IEventEndpoint<TEvent>
    {
        public EventEndpoint(EndpointConfig config) : base(config)
        {
        }

        public Task PublishAsync(TEvent @event,
            CancellationToken token = default(CancellationToken)) => PublishMessageAsync(@event, ChannelKind.None, token);
        
        public Task<Guid> Deliver<TStore>(IUnitOfWork<TStore> uow, TEvent @event, DeliveryOnSuccess? onSuccess = null) 
            => Deliverer(uow, @event, false, DeliveryReply.NoReply, onSuccess ?? Config.GetRequiredService<DeliveryOnSuccess>());

        public Task<Guid> Enqueue<TStore>(IUnitOfWork<TStore> uow, TEvent @event) 
            => Deliverer(uow, @event, false, DeliveryReply.NoReply, Option.None);


        /// <inheritdoc />
        public IDisposable Listen<TChannel>(Func<TEvent, EventContext, CancellationToken, Task> eventListener, 
                TChannel channel = null, Action<ChannelBuilder> configure = null) 
            where TChannel : ChannelKind, IEventChannel 
            => Listen(Config.Channel((ChannelKind)channel ?? ChannelKind.System, false, configure ?? (_ => { })), 
                eventListener, p => new EventContext(p.Sender));
    }
}