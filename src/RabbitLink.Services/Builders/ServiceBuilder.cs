using System;
using System.Linq.Expressions;
using RabbitLink.Services.Descriptions;

namespace RabbitLink.Services
{
    internal class ServiceBuilder : IServiceBuilder
    {
        protected ServiceDescription Description { get; }
        protected ServiceLink Link { get; } 

        public ServiceBuilder(ServiceDescription description, ServiceLink link)
        {
            Description = description;
            Link = link;
            
        }

        public IEventEndpoint Event(string eventName)
            => new EventEndpoint(Description.Events[eventName], Link);
    }

    internal class ServiceBuilder<T> : ServiceBuilder, IServiceBuilder<T>
    {
        public ServiceBuilder(ServiceDescription description, ServiceLink link) : base(description, link)
        {
        }

        public IEventEndpoint<T, TEvent> Event<TEvent>(Expression<Func<T, EventHandler<TEvent>>> selector)
            where TEvent : class => new EventEndpoint<T, TEvent>(Description.Events[selector.GetProperty().Name], Link);
    }
}