using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Astral.Configuration.Builders;
using Astral.Data;
using Astral.Deliveries;
using Astral.Transport;

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
        /// <param name="token">cancellation</param>
        /// <typeparam name="T">service type</typeparam>
        /// <typeparam name="TEvent">event type</typeparam>
        /// <returns>awaitable task</returns>
        Task PublishAsync<TEvent>(Expression<Func<T, IEvent<TEvent>>> selector, TEvent @event, 
            CancellationToken token = default(CancellationToken));


        /// <summary>
        ///     Listen event
        /// </summary>
        /// <param name="selector">event selector</param>
        /// <param name="eventListener">event listener</param>
        /// <param name="channel">channel type, default System</param>
        /// <param name="configure">configure channel, default from config</param>
        /// <typeparam name="T">service type</typeparam>
        /// <typeparam name="TEvent">event type</typeparam>
        /// <returns>dispose to unlisten</returns>
        IDisposable Listen<TEvent>(Expression<Func<T, IEvent<TEvent>>> selector,
            IListener<TEvent, EventContext> eventListener, SubscribeChannel? channel = null, Action<ChannelBuilder> configure = null);


        
        /// <summary>
        /// Deliver event
        /// </summary>
        /// <param name="store">bounded store</param>
        /// <param name="selector">event selector</param>
        /// <param name="event">event</param>
        /// <param name="onSuccess">on success delivery, default from config if present then Delete</param>
        /// <typeparam name="TStore">bounded store type</typeparam>
        /// <typeparam name="TEvent">event type</typeparam>
        /// <returns>awaitable delivery uid</returns>
        Task<Guid> Deliver<TStore, TEvent>(TStore store,
            Expression<Func<T, IEvent<TEvent>>> selector,
            TEvent @event, DeliveryOnSuccess? onSuccess = null)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>;

        /// <summary>
        /// Enqueue event delivery
        /// </summary>
        /// <param name="store">bounded store</param>
        /// <param name="selector">event selector</param>
        /// <param name="event">event</param>
        /// <typeparam name="TStore">bounded store type</typeparam>
        /// <typeparam name="TEvent">event type</typeparam>
        /// <returns>awaitable delivery uid</returns>
        Task<Guid> Enqueue<TStore, TEvent>(TStore store,
            Expression<Func<T, IEvent<TEvent>>> selector,
            TEvent @event)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>;


        /// <summary>
        /// Deliver call
        /// </summary>
        /// <param name="store">bounded store</param>
        /// <param name="selector">call selector</param>
        /// <param name="command">command</param>
        /// <param name="onSuccess">on success delivery, default from config if present then Archive</param>
        /// <param name="replyTo">replay to, default to config if present then System</param>
        /// <typeparam name="TStore">bounded store type</typeparam>
        /// <typeparam name="TCommand">command type</typeparam>
        /// <returns>awaitable delivery uid</returns>
        Task<Guid> Deliver<TStore, TCommand>(TStore store, Expression<Func<T, ICall<TCommand>>> selector,
            TCommand command, DeliveryOnSuccess? onSuccess = null, DeliveryReplyTo? replyTo = null)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>;

        /// <summary>
        /// Enqueue call delivery
        /// </summary>
        /// <param name="store">bounded store</param>
        /// <param name="selector">call selector</param>
        /// <param name="command">command</param>
        /// <param name="replyTo">replay to, default to config if present then System</param>
        /// <typeparam name="TStore">bounded store type</typeparam>
        /// <typeparam name="TCommand">command type</typeparam>
        /// <returns>awaitable delivery uid</returns>
        Task<Guid> Enqueue<TStore, TCommand>(TStore store,
            Expression<Func<T, ICall<TCommand>>> selector, TCommand command, DeliveryReplyTo? replyTo = null)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>;

        /// <summary>
        /// Deliver call
        /// </summary>
        /// <param name="store">bounded store</param>
        /// <param name="selector">call selector</param>
        /// <param name="command">command</param>
        /// <param name="onSuccess">on success delivery, default from config if present then Archive</param>
        /// <param name="replyTo">replay to, default to config if present then System</param>
        /// <typeparam name="TStore">bounded store type</typeparam>
        /// <typeparam name="TRequest">request type</typeparam>
        /// <typeparam name="TResponse">response type</typeparam>
        /// <returns>awaitable delivery uid</returns>
        Task<Guid> Deliver<TStore, TRequest, TResponse>(TStore store, Expression<Func<T, ICall<TRequest, TResponse>>> selector,
            TRequest command, DeliveryOnSuccess? onSuccess = null, DeliveryReplyTo? replyTo = null)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>;

        /// <summary>
        /// Enqueue call delivery
        /// </summary>
        /// <param name="store">bounded store</param>
        /// <param name="selector">call selector</param>
        /// <param name="command">command</param>
        /// <param name="replyTo">replay to, default to config if present then System</param>
        /// <typeparam name="TStore">bounded store type</typeparam>
        /// <typeparam name="TRequest">request type</typeparam>
        /// <typeparam name="TResponse">response type</typeparam>
        /// <returns>awaitable delivery uid</returns>
        Task<Guid> Enqueue<TStore, TRequest, TResponse>(TStore store,
            Expression<Func<T, ICall<TRequest, TResponse>>> selector, TRequest command, DeliveryReplyTo? replyTo = null)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>;

        Task<Guid> DeliverReply<TStore, TCommand>(TStore store,
            Expression<Func<T, ICall<TCommand>>> selector,
            ReplayToInfo replayTo)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>;

        Task<Guid> Send<TCommand>(Expression<Func<T, ICall<TCommand>>> selector, TCommand command,
            ResponseTo responseTo = null, CancellationToken cancellation = default(CancellationToken));

        Task Response<TCommand>(Expression<Func<T, ICall<TCommand>>> selector,
            ReplayToInfo replayTo, CancellationToken cancellation = default(CancellationToken));
    }

    public interface IBusService
    {


    }
}