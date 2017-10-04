using Astral.Contracts;
using RabbitLink.Services.Descriptions;

namespace RabbitLink.Services
{
    internal class EventEndpoint : IEventEndpoint
    {
        protected EventDescription Description { get; }
        protected ServiceLink Link { get; }

        public EventEndpoint(EventDescription description, ServiceLink link)
        {
            Description = description;
            Link = link;
        }

        ILinkEventPublisher IEventEndpoint.Publisher => new EventPublisher(Description, Link);

        ILinkEventConsumer IEventEndpoint.Consumer => new EventConsumer(Link, Description);
        
    }

    internal class EventEndpoint<TService, TEvent> : EventEndpoint, IEventEndpoint<TService, TEvent> 
        where TEvent : class
    {
        

        public EventEndpoint(EventDescription description, ServiceLink link) : base(description, link)
        {
        }

        ILinkEventPublisher<TService, TEvent> IEventEndpoint<TService, TEvent>.Publisher => new EventPublisher<TService, TEvent>(Description, Link);

        ILinkEventConsumer<TService, TEvent> IEventEndpoint<TService, TEvent>.Consumer => new EventConsumer<TService, TEvent>(Link, Description);
        
    }
}