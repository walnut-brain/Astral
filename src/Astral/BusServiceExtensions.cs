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
        


        /// <summary>
        ///     Listen event
        /// </summary>
        /// <param name="service">bus service</param>
        /// <param name="selector">event selector</param>
        /// <param name="eventListener">event listener</param>
        /// <param name="options">listen options</param>
        /// <typeparam name="TService">service type</typeparam>
        /// <typeparam name="TTransport">transport type</typeparam>
        /// <typeparam name="TEvent">event type</typeparam>
        /// <returns>dispose to unlisten</returns>
        /*public static IDisposable Listen<TService, TTransport, TEvent>(this BusService<TTransport, TService> service,
            Expression<Func<TService, IEvent<TEvent>>> selector,
            IEventListener<TEvent> eventListener,
            EventListenOptions options = null)
            where TTransport : class, IEventTransport where TService : class
        {
            return Operations.ListenEvent(service.Logger, service.Config.Endpoint(selector), service.Transport.Subscribe,
                eventListener,
                options);
        }

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