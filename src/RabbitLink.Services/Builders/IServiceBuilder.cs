using System;
using System.Linq.Expressions;

namespace RabbitLink.Services
{
    public interface IServiceBuilder<TService> : IServiceBuilder
    {
        IEventEndpoint<TService, TEvent> Event<TEvent>(Expression<Func<TService, EventHandler<TEvent>>> selector) where TEvent : class;
    }

    public interface IServiceBuilder
    {
        IEventEndpoint Event(string eventName);
    }
}