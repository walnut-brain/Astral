using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Astral.Data;
using Astral.Internals;
using Astral.Transport;
using Microsoft.Extensions.DependencyInjection;

namespace Astral
{
    /// <summary>
    ///     Transport specific bus service extensions
    /// </summary>
    public static class BusServiceExtensions
    {
        


        /*

        public static Action EnqueueManual<TService, TTransport, TStore, TEvent>(
            this BusService<TTransport, TService> service, IDeliveryDataService<TStore> dataService,
            Expression<Func<TService, IEvent<TEvent>>> selector, TEvent @event,
            EventPublishOptions options = null)
            where TStore : IStore<TStore>
            where TTransport : class, IEventTransport
            where TService : class
        {
            return Operations.EnqueueManual(service.Logger, dataService, service.Config.Endpoint(selector),
                service.Transport.PreparePublish, @event,
                service.Provider.GetRequiredService<DeliveryManager<TStore>>());
        }

        public static void Deliver<TService, TTransport, TStore, TEvent>(
            this BusService<TTransport, TService> service, IDeliveryDataService<TStore> dataService, 
            Expression<Func<TService, IEvent<TEvent>>> selector, TEvent @event,
            EventPublishOptions options = null)
            where TStore : IStore<TStore>, IRegisterAfterCommit
            where TTransport : class, IEventTransport
            where TService : class
        {
            service.Provider.GetRequiredService<TStore>().RegisterAfterCommit(service.EnqueueManual(dataService, selector, @event, options));
        }*/
    }
}