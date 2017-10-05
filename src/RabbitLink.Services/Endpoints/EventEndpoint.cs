using RabbitLink.Services.Descriptions;

namespace RabbitLink.Services
{

    internal class EventEndpoint<TService, TEvent> : IEventEndpoint<TService, TEvent> 
        where TEvent : class
    {
        private EventDescription Description { get; }
        private ServiceLink Link { get; }

        public EventEndpoint(EventDescription description, ServiceLink link) 
        {
            Description = description;
            Link = link;
        }

        ILinkEventPublisher<TService, TEvent> IEventEndpoint<TService, TEvent>.Publisher => new EventPublisher<TService, TEvent>(Description, Link);

        ILinkEventConsumer<TService, TEvent> IEventEndpoint<TService, TEvent>.Consumer => new EventConsumer<TService, TEvent>(Link, Description);
        
    }
}