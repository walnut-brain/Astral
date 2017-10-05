using System;
using System.Threading;
using System.Threading.Tasks;
using RabbitLink.Producer;
using RabbitLink.Services.Descriptions;

namespace RabbitLink.Services
{
    internal class CallConsumer<TService, TArg, TResult> : ILinkCallConsumer<TService, TArg, TResult> 
        where TResult : class 
        where TArg : class
    {
        protected ServiceLink Link { get; }
        protected CallDescription Description { get; }
        protected Lazy<ILinkProducer> Producer { get; }
        protected bool ExchangePassive { get; }
        protected bool ConfirmsMode { get; }

        public CallConsumer(ServiceLink link, CallDescription description)
        {
            Link = link;
            Description = description;
            Producer = new Lazy<ILinkProducer>(() => Link.GetOrAddProducer(Description.RequestExchange.Name ?? "", ConfirmsMode, CreateProducer));
        }

        private ILinkProducer CreateProducer()
        {
            return
                Link
                    .Producer
                    .ConfirmsMode(ConfirmsMode)
                    .Exchange(cfg =>
                        string.IsNullOrWhiteSpace(Description.RequestExchange.Name)
                            ? cfg.ExchangeDeclareDefault()
                            : ExchangePassive
                                ? cfg.ExchangeDeclarePassive(Description.RequestExchange.Name)
                                : cfg.ExchangeDeclare(Description.RequestExchange.Name,
                                    Description.RequestExchange.Type, Description.RequestExchange.Durable, Description.RequestExchange.AutoDelete,
                                    Description.RequestExchange.Alternate, Description.RequestExchange.Delayed))
                    .Serializer(Link.PayloadManager.GetSerializer(Description.ContentType))
                    .Build();
        }
        
        public IDisposable Process(Func<TArg, CancellationToken, Task<TResult>> processor)
        {
            
        }
    }

    internal class CallConsumer<TService, TArg> : CallConsumer<TService, TArg, Unit>,  ILinkCallConsumer<TService, TArg> 
        where TArg : class
    {
        public CallConsumer(ServiceLink link, CallDescription description) : base(link, description)
        {
        }
    }
}