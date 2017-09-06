using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Astral.Data;
using Astral.Deliveries;

namespace Astral
{
    public interface IBusService<T> : IBusService
        where T : class
    {
        /// <summary>
        ///     Publish event asynchronius
        /// </summary>
        /// <param name="selector">event selector expression</param>
        /// <param name="event">published event</param>
        /// <param name="options">publicashion options</param>
        /// <param name="token">cancellation</param>
        /// <typeparam name="T">service type</typeparam>
        /// <typeparam name="TEvent">event type</typeparam>
        /// <returns>awaitable task</returns>
        Task PublishAsync<TEvent>(Expression<Func<T, IEvent<TEvent>>> selector, TEvent @event,
            EventPublishOptions options = null, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Deliver event with bounded store
        /// </summary>
        /// <param name="store">store instance</param>
        /// <param name="selector">event selector expression</param>
        /// <param name="event">even to deliver</param>
        /// <param name="options">delivery options</param>
        /// <typeparam name="TStore">store type</typeparam>
        /// <typeparam name="TEvent">event type</typeparam>
        /// <returns>awaitable delivery guid</returns>
        Task<Guid> Deliver<TStore, TEvent>(TStore store, Expression<Func<T, IEvent<TEvent>>> selector,
            TEvent @event, DeliveryOptions options = null)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>;

        /// <summary>
        ///     Listen event
        /// </summary>
        /// <param name="selector">event selector</param>
        /// <param name="eventListener">event listener</param>
        /// <param name="options">listen options</param>
        /// <typeparam name="T">service type</typeparam>
        /// <typeparam name="TEvent">event type</typeparam>
        /// <returns>dispose to unlisten</returns>
        IDisposable Listen<TEvent>(Expression<Func<T, IEvent<TEvent>>> selector,
            IEventListener<TEvent> eventListener, EventListenOptions options = null);
    }

    public interface IBusService
    {


    }
}