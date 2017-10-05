using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Astral.Links;
using RabbitLink.Messaging;
using RabbitLink.Producer;
using RabbitLink.Services.Descriptions;
using RabbitLink.Services.Exceptions;
using RabbitLink.Topology;

namespace RabbitLink.Services
{
    internal class EventPublisher<TService, TEvent> :
        ILinkEventPublisher<TService, TEvent>
        where TEvent : class
    {
        private EventDescription Description { get; }
        private ServiceLink Link { get; }
        private bool Passive { get; }
        private bool ConfirmMode { get; }
        private string Name { get; }
        private Lazy<ILinkProducer> Producer { get; }
        
        public EventPublisher(EventDescription description, ServiceLink link, bool passive = false, bool confirmMode = true,
            string name = null) 
        {
            if(description.Type != typeof(TEvent))
                throw new InvalidConfigurationException("Invalid configuration - type in description different from generic type");
            Description = description;
            Link = link;
            Passive = passive;
            ConfirmMode = confirmMode;
            Name = name ?? description.Exchange.Name;
            Producer = new Lazy<ILinkProducer>(() => Link.GetOrAddProducer(Name, ConfirmMode, CreateProducer));
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

        private Task PublishAsync(TEvent message, CancellationToken token)
        {
            var msg = new LinkPublishMessage<TEvent>(message, publishProperties: new LinkPublishProperties
            {
                RoutingKey =
                    Description.Exchange.Type == LinkExchangeType.Fanout ? null :
                        Description.RoutingKey ?? Description.RoutingKeyExtractor(message)
            });
            return Producer.Value.PublishAsync(msg, token);
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