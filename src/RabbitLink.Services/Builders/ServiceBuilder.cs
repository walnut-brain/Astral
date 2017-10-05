using System;
using System.Linq.Expressions;
using RabbitLink.Services.Descriptions;

namespace RabbitLink.Services
{
    internal class ServiceBuilder<T> : IServiceBuilder<T>
    {
        private ServiceDescription Description { get; }
        private ServiceLink Link { get; }
        
        public ServiceBuilder(ServiceDescription description, ServiceLink link) 
        {
            Description = description;
            Link = link;
        }

        public IEventEndpoint<T, TEvent> Event<TEvent>(Expression<Func<T, EventHandler<TEvent>>> selector)
            where TEvent : class 
            => new EventEndpoint<T, TEvent>(Description.Events[selector.GetProperty().Name], Link);

        public ICallEndpoint<T, TArg> Call<TArg>(Expression<Func<T, Action<TArg>>> selector) 
            where TArg : class
            => new CallEndpoint<T, TArg>(Link, Description.Calls[selector.GetProperty().Name]);

        public ICallEndpoint<T, TArg, TResult> Call<TArg, TResult>(Expression<Func<T, Func<TArg, TResult>>> selector) 
            where TArg : class 
            where TResult : class
            => new CallEndpoint<T, TArg, TResult>(Link, Description.Calls[selector.GetProperty().Name]);
    }
}