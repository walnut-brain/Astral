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


        /// <summary>
        /// Save delivery event with bounded store, not send event
        /// </summary>
        /// <param name="store">store instance</param>
        /// <param name="selector">event selector expression</param>
        /// <param name="event">event to deliver</param>
        /// <param name="messageTtl">event ttl</param>
        /// <typeparam name="TStore">store type</typeparam>
        /// <typeparam name="TEvent">event type</typeparam>
        /// <returns>awaitable delivery guid</returns>
        Task<Guid> SaveDelivery<TStore, TEvent>(TStore store,
            Expression<Func<T, IEvent<TEvent>>> selector,
            TEvent @event, TimeSpan? messageTtl = null)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>;


        /// <summary>
        /// Deliver command
        /// </summary>
        /// <param name="store">store instance</param>
        /// <param name="selector">command selector</param>
        /// <param name="command">command</param>
        /// <param name="target">target system</param>
        /// <param name="messageTtl">command ttl</param>
        /// <param name="onSuccess">on success delivery </param>
        /// <param name="replyTo">reply to mode</param>
        /// <typeparam name="TStore">store type</typeparam>
        /// <typeparam name="TCommand">command type</typeparam>
        /// <returns>awaitable delivery guid</returns>
        Task<Guid> Deliver<TStore, TCommand>(TStore store, Expression<Func<T, ICall<TCommand>>> selector,
            TCommand command,
            string target = null, TimeSpan? messageTtl = null, DeliveryOnSuccess onSuccess = null,
            DeliveryReplyTo replyTo = null)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>;

        /// <summary>
        /// Save delivery for command
        /// </summary>
        /// <param name="store">store instance</param>
        /// <param name="selector">command selector</param>
        /// <param name="command">command</param>
        /// <param name="target">target system</param>
        /// <param name="messageTtl">command ttl</param>
        /// <param name="replyTo">reply to mode</param>
        /// <typeparam name="TStore">store type</typeparam>
        /// <typeparam name="TCommand">command type</typeparam>
        /// <returns>awaitable delivery guid</returns>
        Task<Guid> SaveDelivery<TStore, TCommand>(TStore store,
            Expression<Func<T, ICall<TCommand>>> selector, TCommand command,
            string target = null, TimeSpan? messageTtl = null, DeliveryReplyTo replyTo = null)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>;
        
        /// <summary>
        /// Deliver command
        /// </summary>
        /// <param name="store">store instance</param>
        /// <param name="selector">command selector</param>
        /// <param name="command">command</param>
        /// <param name="target">target system</param>
        /// <param name="messageTtl">command ttl</param>
        /// <param name="onSuccess">on success delivery </param>
        /// <param name="replyTo">reply to mode</param>
        /// <typeparam name="TStore">store type</typeparam>
        /// <typeparam name="TCommand">command type</typeparam>
        /// <returns>awaitable delivery guid</returns>
        Task<Guid> Deliver<TStore, TRequest, TResponse>(TStore store, Expression<Func<T, ICall<TRequest, TResponse>>> selector,
            TRequest request,
            string target = null, TimeSpan? messageTtl = null, DeliveryOnSuccess onSuccess = null,
            DeliveryReplyTo replyTo = null)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>;

        /// <summary>
        /// Save delivery for command
        /// </summary>
        /// <param name="store">store instance</param>
        /// <param name="selector">command selector</param>
        /// <param name="command">command</param>
        /// <param name="target">target system</param>
        /// <param name="messageTtl">command ttl</param>
        /// <param name="replyTo">reply to mode</param>
        /// <typeparam name="TStore">store type</typeparam>
        /// <typeparam name="TCommand">command type</typeparam>
        /// <returns>awaitable delivery guid</returns>
        Task<Guid> SaveDelivery<TStore, TRequest, TResponse>(TStore store,
            Expression<Func<T, ICall<TRequest, TResponse>>> selector, TRequest command,
            string target = null, TimeSpan? messageTtl = null, DeliveryReplyTo replyTo = null)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>;
    }

    public interface IBusService
    {


    }
}