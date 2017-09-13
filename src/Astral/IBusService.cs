using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Astral.Configuration.Builders;
using Astral.Data;
using Astral.Deliveries;
using Astral.Transport;
using FunEx.Monads;

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
        IDisposable Listen<TEvent, TChannel>(Expression<Func<T, IEvent<TEvent>>> selector,
            IListener<TEvent, EventContext> eventListener, TChannel channel = null, Action<ChannelBuilder> configure = null)
            where TChannel : ChannelKind, ChannelKind.IEventChannel;


        
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
            TCommand command, DeliveryOnSuccess? onSuccess = null, ChannelKind.IDeliveryReply replyTo = null)
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
            Expression<Func<T, ICall<TCommand>>> selector, TCommand command, ChannelKind.IDeliveryReply replyTo = null)
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
            TRequest command, DeliveryOnSuccess? onSuccess = null, ChannelKind.IDeliveryReply replyTo = null)
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
            Expression<Func<T, ICall<TRequest, TResponse>>> selector, TRequest command, ChannelKind.IDeliveryReply replyTo = null)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>;

        Task<Guid> DeliverReply<TStore, TCommand>(TStore store,
            Expression<Func<T, ICall<TCommand>>> selector, ChannelKind.ReplyChannelKind replayTo)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>;

        Task<Guid> Send<TCommand>(Expression<Func<T, ICall<TCommand>>> selector, TCommand command,
            ChannelKind.IDeliveryReply responseTo = null, CancellationToken cancellation = default(CancellationToken));

        Task Response<TCommand>(Expression<Func<T, ICall<TCommand>>> selector,
            ChannelKind.ReplyChannelKind replayTo, CancellationToken cancellation = default(CancellationToken));

        Task<Guid> DeliverReply<TStore, TRequest, TReplay>(TStore store,
            Expression<Func<T, ICall<TRequest, TReplay>>> selector, TReplay replay,
            ChannelKind.ReplyChannelKind replayTo)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>;

        IDisposable ListenReply<TCommand>(Expression<Func<T, ICall<TCommand>>> selector,
            IListener<Result<ValueTuple>, ResponseContext> listener,
            ChannelKind.IDeliveryReply replyTo = null, Action<ChannelBuilder> configure = null);

        IDisposable ListenReply<TRequest, TResponse>(Expression<Func<T, ICall<TRequest, TResponse>>> selector,
            IListener<Result<TResponse>, ResponseContext> listener,
            ChannelKind.IDeliveryReply replyTo = null, Action<ChannelBuilder> configure = null);

        Task Response<TRequest, TResponse>(Expression<Func<T, ICall<TRequest, TResponse>>> selector,
            TResponse response, ChannelKind.ReplyChannelKind replayTo, CancellationToken cancellation = default(CancellationToken));

        IDisposable ListenRequest<TCommand>(Expression<Func<T, ICall<TCommand>>> selector,
            IListener<TCommand, RequestContext> listener, Action<ChannelBuilder> configure = null);

        IDisposable ListenRequest<TRequest, TResponse>(Expression<Func<T, ICall<TRequest, TResponse>>> selector,
            IListener<TRequest, RequestContext<TResponse>> listener, Action<ChannelBuilder> configure = null);

        Task<TResponse> Call<TRequest, TResponse>(
            Expression<Func<T, ICall<TRequest, TResponse>>> selector,
            TRequest request, TimeSpan? timeout = null);

        Task Call<TCommand>(
            Expression<Func<T, ICall<TCommand>>> selector,
            TCommand request, TimeSpan? timeout = null);

        Task ResponseFault<TCommand>(Expression<Func<T, ICall<TCommand>>> selector,
            RequestFault fault, ChannelKind.ReplyChannelKind replayTo, CancellationToken cancellation = default(CancellationToken));

        Task<Guid> Send<TRequest, TResponse>(Expression<Func<T, ICall<TRequest, TResponse>>> selector, TRequest command,
            ChannelKind.IDeliveryReply responseTo = null, CancellationToken cancellation = default(CancellationToken));

        Task ResponseFault<TRequest, TResponse>(Expression<Func<T, ICall<TRequest, TResponse>>> selector,
            RequestFault fault, ChannelKind.ReplyChannelKind replayTo, CancellationToken cancellation = default(CancellationToken));

        Task<Guid> DeliverFaultReply<TStore, TCommand>(TStore store,
            Expression<Func<T, ICall<TCommand>>> selector, RequestFault fault,
            ChannelKind.ReplyChannelKind replayTo)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>;

        Task<Guid> DeliverFaultReply<TStore, TRequest, TReplay>(TStore store,
            Expression<Func<T, ICall<TRequest, TReplay>>> selector, RequestFault fault,
            ChannelKind.ReplyChannelKind replayTo)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>;

        IDisposable HandleCall<TRequest, TResponse>(
            Expression<Func<T, ICall<TRequest, TResponse>>> selector,
            Func<TRequest, CancellationToken, Task<TResponse>> handler);

        IDisposable HandleCall<TCommand>(
            Expression<Func<T, ICall<TCommand>>> selector,
            Func<TCommand, CancellationToken, Task> handler);
    }

    public interface IBusService : IDisposable
    {


    }
}