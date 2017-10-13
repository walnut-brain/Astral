using System;
using System.Linq.Expressions;
using Astral.Liaison;
using Astral.RabbitLink.Descriptions;
using Astral.RabbitLink.Internals;
using Astral.Schema;
using Astral.Schema.RabbitMq;

namespace Astral.RabbitLink
{
    internal class ServiceBuilder<T> : IServiceBuilder<T>
    {
        private IServiceSchema Schema { get; }
        private ServiceLink Link { get; }

        IServiceSchema IServiceBuilder<T>.Schema => Schema;
        

        public ServiceBuilder(IServiceSchema schema, ServiceLink link) 
        {
            Schema = schema;
            Link = link;
        }

        public IEventEndpoint<T, TEvent> Event<TEvent>(Expression<Func<T, EventHandler<TEvent>>> selector)
            => new EventEndpoint<T, TEvent>(Link, (IRabbitMqEventSchema) Schema.EventByCodeName(selector.GetProperty().Name));

        public ICallEndpoint<T, TArg> Call<TArg>(Expression<Func<T, Action<TArg>>> selector) 
            => new CallEndpoint<T, TArg>(Link, (IRabbitMqCallSchema) Schema.CallByCodeName(selector.GetProperty().Name));

        public ICallEndpoint<T, TArg, TResult> Call<TArg, TResult>(Expression<Func<T, Func<TArg, TResult>>> selector) 
            => new CallEndpoint<T, TArg, TResult>(Link, (IRabbitMqCallSchema) Schema.CallByCodeName(selector.GetProperty().Name));
        
        public IRequestEndpoint<T, TArg, TResult> Request<TArg, TResult>(Expression<Func<T, Func<TArg, TResult>>> selector)
            => new RequestEndpoint<T, TArg, TResult>(Link, (IRabbitMqCallSchema) Schema.CallByCodeName(selector.GetProperty().Name));
        
        public IRequestEndpoint<T, TArg, RpcOk> Request<TArg>(Expression<Func<T, Action<TArg>>> selector)
            => new RequestEndpoint<T, TArg, RpcOk>(Link, (IRabbitMqCallSchema) Schema.CallByCodeName(selector.GetProperty().Name));

        public IResponseEndpoint<T, TArg, TResult> Response<TArg, TResult>(Expression<Func<T, Func<TArg, TResult>>> selector)
            => new ResponseEndpoint<T, TArg, TResult>(Link, (IRabbitMqCallSchema) Schema.CallByCodeName(selector.GetProperty().Name));

        public IResponseEndpoint<T, TArg, RpcOk> Response<TArg>(Expression<Func<T, Action<TArg>>> selector)
            => new ResponseEndpoint<T, TArg, RpcOk>(Link, (IRabbitMqCallSchema) Schema.CallByCodeName(selector.GetProperty().Name));
    }
}