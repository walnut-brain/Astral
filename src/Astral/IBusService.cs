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


        Task<Guid> Deliver<TStore, TEvent>(TStore store, Expression<Func<T, IEvent<TEvent>>> selector,
            TEvent @event, DeliveryOptions options = null)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>;
    }

    public interface IBusService
    {


    }
}