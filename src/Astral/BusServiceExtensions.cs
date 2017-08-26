using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Astral.Core;
using Astral.Internals;
using Astral.Transport;

namespace Astral
{
    /// <summary>
    ///     Transport specific bus service extensions
    /// </summary>
    public static class BusServiceExtensions
    {
        /// <summary>
        ///     Publish event asynchronius
        /// </summary>
        /// <param name="service">bus service to publish</param>
        /// <param name="selector">event selector expression</param>
        /// <param name="event">published event</param>
        /// <param name="options">publicashion options</param>
        /// <typeparam name="TService">service type</typeparam>
        /// <typeparam name="TTransport">transport type</typeparam>
        /// <typeparam name="TEvent">event type</typeparam>
        /// <returns>awaitable task</returns>
        public static Task PublishAsync<TService, TTransport, TEvent>(this BusService<TTransport, TService> service,
            Expression<Func<TService, IEvent<TEvent>>> selector, TEvent @event,
            EventPublishOptions options = null)
            where TTransport : class, IEventTransport where TService : class
        {
            return Operations.PublishEventAsync(service.Logger, service.Config.Endpoint(selector), service.Transport,
                @event,
                options);
        }


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
        public static IDisposable Listen<TService, TTransport, TEvent>(this BusService<TTransport, TService> service,
            Expression<Func<TService, IEvent<TEvent>>> selector,
            IEventListener<TEvent> eventListener,
            EventListenOptions options = null)
            where TTransport : class, IEventTransport where TService : class
        {
            return Operations.ListenEvent(service.Logger, service.Config.Endpoint(selector), service.Transport,
                eventListener,
                options);
        }
    }
}