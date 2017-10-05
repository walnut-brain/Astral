using System;
using System.Linq.Expressions;

namespace RabbitLink.Services
{
    public interface IServiceBuilder<TService> 
    {
        IEventEndpoint<TService, TEvent> Event<TEvent>(Expression<Func<TService, EventHandler<TEvent>>> selector) 
            where TEvent : class;
        
        ICallEndpoint<TService, TArg> Call<TArg>(Expression<Func<TService, Action<TArg>>> selector) 
            where TArg : class;

        ICallEndpoint<TService, TArg, TResult> Call<TArg, TResult>(Expression<Func<TService, Func<TArg, TResult>>> selector) 
            where TArg : class 
            where TResult : class;
    }

    
}