using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Astral.Contracts;
using RabbitLink.Messaging;
using RabbitLink.Producer;
using RabbitLink.Services.Descriptions;
using RabbitLink.Services.Exceptions;
using RabbitLink.Topology;

namespace RabbitLink.Services
{
    internal class EventPublisher : ILinkEventPublisher
    {
        protected EventDescription Description { get; }
        protected ServiceLink Link { get; }
        protected bool Passive { get; }
        protected bool ConfirmMode { get; }
        protected string Name { get; }
        private Lazy<ILinkProducer> Producer { get; }
        
        private Func<object, CancellationToken, Task> UntypedPublishAsync { get; }

        public EventPublisher(EventDescription description, ServiceLink link, bool passive = false, bool confirmMode = true, string name = null)
        {
            Description = description;
            Link = link;
            Passive = passive;
            ConfirmMode = confirmMode;
            Name = name ?? description.Exchange.Name;
            Producer = new Lazy<ILinkProducer>(() => Link.GetOrAddProducer(Name, ConfirmMode, CreateProducer));
            var method = typeof(EventPublisher).GetMethod(nameof(PublishAsync), BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(Description.Type);
            UntypedPublishAsync = (o, t) => (Task) method.Invoke(this, new[] {o, t});
        }

        private ILinkProducer CreateProducer()
        {
            return
                Link
                    .Producer
                    .ConfirmsMode(ConfirmMode)
                    .Exchange(cfg => Passive
                        ? cfg.ExchangeDeclarePassive(Description.Exchange.Name)
                        : cfg.ExchangeDeclare(Description.Exchange.Name,
                            Description.Exchange.Type, Description.Exchange.Durable, Description.Exchange.AutoDelete,
                            Description.Exchange.Alternate, Description.Exchange.Delayed))
                    .Serializer(Link.PayloadManager.GetSerializer(Description.ContentType))
                    .Build();
        }

        protected Task PublishAsync<TEvent>(TEvent message, CancellationToken token) where TEvent : class
        {
            var msg = new LinkPublishMessage<TEvent>(message, publishProperties: new LinkPublishProperties
            {
                RoutingKey =
                    Description.Exchange.Type == LinkExchangeType.Fanout ? null :
                    Description.RoutingKey ?? Description.RoutingKeyExtractor(message)
            });
            return Producer.Value.PublishAsync(msg, token);
        }

        Task IEventPublisher.PublishAsync(object message, CancellationToken token)
            => UntypedPublishAsync(message, token);

        ILinkEventPublisher ILinkEventPublisher.DeclarePassive(bool value)
            => value == Passive ? this : new EventPublisher(Description, Link, value, ConfirmMode, Name);

        ILinkEventPublisher ILinkEventPublisher.ConfirmMode(bool value)
            => value == ConfirmMode ? this : new EventPublisher(Description, Link, Passive, value, Name);

        ILinkEventPublisher ILinkEventPublisher.Named(string name)
            => name == Name ? this : new EventPublisher(Description, Link, Passive, ConfirmMode, Name);
    }

    internal class EventPublisher<TService, TEvent> : EventPublisher,
        ILinkEventPublisher<TService, TEvent>
        where TEvent : class
    {
        public EventPublisher(EventDescription description, ServiceLink link, bool passive = false, bool confirmMode = true,
            string name = null) : base(description, link, passive, confirmMode, name)
        {
            if(description.Type != typeof(TEvent))
                throw new InvalidConfigurationException("Invalid configuration - type in description different from generic type");
        }

        Task IEventPublisher<TEvent>.PublishAsync(TEvent message, CancellationToken token)  
            => PublishAsync(message, token);
        
        ILinkEventPublisher<TService, TEvent> ILinkEventPublisher<TService, TEvent>.DeclarePassive(bool value)
            => value == Passive ? this : new EventPublisher<TService, TEvent>(Description, Link, value, ConfirmMode, Name);

        ILinkEventPublisher<TService, TEvent> ILinkEventPublisher<TService, TEvent>.ConfirmMode(bool value)
            => value == ConfirmMode ? this : new EventPublisher<TService, TEvent>(Description, Link, Passive, value, Name);

        ILinkEventPublisher<TService, TEvent> ILinkEventPublisher<TService, TEvent>.Named(string name)
            => name == Name ? this : new EventPublisher<TService, TEvent>(Description, Link, Passive, ConfirmMode, Name);
    }
}