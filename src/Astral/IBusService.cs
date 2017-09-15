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
            Func<TEvent, EventContext, CancellationToken, Task> eventListener, TChannel channel = null, Action<ChannelBuilder> configure = null)
            where TChannel : ChannelKind, IEventChannel;


        
        /// <summary>
        /// Deliver event
        /// </summary>
        /// <param name="uow">current unit of work</param>
        /// <param name="selector">event selector</param>
        /// <param name="event">event</param>
        /// <param name="onSuccess">on success delivery, default from config if present then Delete</param>
        /// <typeparam name="TStore">store type</typeparam>
        /// <typeparam name="TEvent">event type</typeparam>
        /// <returns>awaitable delivery uid</returns>
        Task<Guid> Deliver<TStore, TEvent>(IUnitOfWork<TStore> uow,
            Expression<Func<T, IEvent<TEvent>>> selector,
            TEvent @event, DeliveryOnSuccess? onSuccess = null);

        /// <summary>
        /// Enqueue event delivery
        /// </summary>
        /// <param name="uow">current unit of work</param>
        /// <param name="selector">event selector</param>
        /// <param name="event">event</param>
        /// <typeparam name="TStore">store type</typeparam>
        /// <typeparam name="TEvent">event type</typeparam>
        /// <returns>awaitable delivery uid</returns>
        Task<Guid> Enqueue<TStore, TEvent>(IUnitOfWork<TStore> uow,
            Expression<Func<T, IEvent<TEvent>>> selector,
            TEvent @event);


        /// <summary>
        /// Deliver call
        /// </summary>
        /// <param name="uow">current unit of work</param>
        /// <param name="selector">call selector</param>
        /// <param name="command">command</param>
        /// <param name="onSuccess">on success delivery, default from config if present then Archive</param>
        /// <param name="replyTo">replay to, default to config if present then System</param>
        /// <typeparam name="TStore">store type</typeparam>
        /// <typeparam name="TCommand">command type</typeparam>
        /// <returns>awaitable delivery uid</returns>
        Task<Guid> Deliver<TStore, TCommand>(IUnitOfWork<TStore> uow, Expression<Func<T, ICall<TCommand>>> selector,
            TCommand command, DeliveryOnSuccess? onSuccess = null, ChannelKind.DurableChannel replyTo = null);


        /// <summary>
        /// Enqueue call delivery
        /// </summary>
        /// <param name="uow">current unit of work</param>
        /// <param name="selector">call selector</param>
        /// <param name="command">command</param>
        /// <param name="replyTo">replay to, default to config if present then System</param>
        /// <typeparam name="TStore">store type</typeparam>
        /// <typeparam name="TCommand">command type</typeparam>
        /// <returns>awaitable delivery uid</returns>
        Task<Guid> Enqueue<TStore, TCommand>(IUnitOfWork<TStore> uow,
            Expression<Func<T, ICall<TCommand>>> selector, TCommand command, ChannelKind.DurableChannel replyTo = null);

        /// <summary>
        /// Deliver call
        /// </summary>
        /// <param name="uow">current unit of work</param>
        /// <param name="selector">call selector</param>
        /// <param name="command">command</param>
        /// <param name="onSuccess">on success delivery, default from config if present then Archive</param>
        /// <param name="replyTo">replay to, default to config if present then System</param>
        /// <typeparam name="TStore">bstore type</typeparam>
        /// <typeparam name="TRequest">request type</typeparam>
        /// <typeparam name="TResponse">response type</typeparam>
        /// <returns>awaitable delivery uid</returns>
        Task<Guid> Deliver<TStore, TRequest, TResponse>(IUnitOfWork<TStore> uow, Expression<Func<T, ICall<TRequest, TResponse>>> selector,
            TRequest command, DeliveryOnSuccess? onSuccess = null, ChannelKind.DurableChannel replyTo = null);

        /// <summary>
        /// Enqueue call delivery
        /// </summary>
        /// <param name="uow">current unit of work</param>
        /// <param name="selector">call selector</param>
        /// <param name="command">command</param>
        /// <param name="replyTo">replay to, default to config if present then System</param>
        /// <typeparam name="TStore">store type</typeparam>
        /// <typeparam name="TRequest">request type</typeparam>
        /// <typeparam name="TResponse">response type</typeparam>
        /// <returns>awaitable delivery uid</returns>
        Task<Guid> Enqueue<TStore, TRequest, TResponse>(IUnitOfWork<TStore> uow,
            Expression<Func<T, ICall<TRequest, TResponse>>> selector, TRequest command, ChannelKind.DurableChannel replyTo = null);

        
        /// <summary>
        /// Deliver command reply
        /// </summary>
        /// <param name="uow">current unit of work</param>
        /// <param name="selector">command selector</param>
        /// <param name="replayTo">replay to</param>
        /// <typeparam name="TStore">store type</typeparam>
        /// <typeparam name="TCommand">command type</typeparam>
        /// <returns>awaitable delivery id</returns>
        Task<Guid> DeliverResponse<TStore, TCommand>(IUnitOfWork<TStore> uow,
            Expression<Func<T, ICall<TCommand>>> selector, ChannelKind.ReplyChannel replayTo);

        /// <summary>
        /// Send command
        /// </summary>
        /// <param name="selector">command selector</param>
        /// <param name="command">command</param>
        /// <param name="responseTo">response to</param>
        /// <param name="cancellation">cancellation token</param>
        /// <typeparam name="TCommand">command type</typeparam>
        /// <returns>awaitable request id</returns>
        Task<Guid> Send<TCommand>(Expression<Func<T, ICall<TCommand>>> selector, TCommand command,
            ChannelKind.RespondableChannel responseTo = null, CancellationToken cancellation = default(CancellationToken));

        /// <summary>
        /// Response to command
        /// </summary>
        /// <param name="selector">command selector</param>
        /// <param name="replyTo">reply to</param>
        /// <param name="cancellation">cancellation token</param>
        /// <typeparam name="TCommand">command type</typeparam>
        /// <returns>awaitable</returns>
        Task Response<TCommand>(Expression<Func<T, ICall<TCommand>>> selector,
            ChannelKind.ReplyChannel replyTo, CancellationToken cancellation = default(CancellationToken));

        /// <summary>
        /// Deliverty call response
        /// </summary>
        /// <param name="uow">current unit of work</param>
        /// <param name="selector">call selector</param>
        /// <param name="response">response</param>
        /// <param name="replyTo">reply to</param>
        /// <typeparam name="TStore">store type</typeparam>
        /// <typeparam name="TRequest">request type</typeparam>
        /// <typeparam name="TResponse">response type</typeparam>
        /// <returns></returns>
        Task<Guid> DeliverResponse<TStore, TRequest, TResponse>(IUnitOfWork<TStore> uow,
            Expression<Func<T, ICall<TRequest, TResponse>>> selector, TResponse response,
            ChannelKind.ReplyChannel replyTo);

        /// <summary>
        /// Listen response from command
        /// </summary>
        /// <param name="selector">command selector</param>
        /// <param name="listener">response listener</param>
        /// <param name="replyFrom">response channel</param>
        /// <param name="configure">configure channel</param>
        /// <typeparam name="TCommand">command type</typeparam>
        /// <returns>dispose for unlisten</returns>
        IDisposable ListenResponse<TCommand>(Expression<Func<T, ICall<TCommand>>> selector,
            Func<Result<ValueTuple>, ResponseContext, CancellationToken, Task> listener,
            ChannelKind.DurableChannel replyFrom = null, Action<ChannelBuilder> configure = null);

        IDisposable ListenResponse<TRequest, TResponse>(Expression<Func<T, ICall<TRequest, TResponse>>> selector,
            Func<Result<TResponse>, ResponseContext, CancellationToken, Task> listener,
            ChannelKind.DurableChannel replyFrom = null, Action<ChannelBuilder> configure = null);

        Task Response<TRequest, TResponse>(Expression<Func<T, ICall<TRequest, TResponse>>> selector,
            TResponse response, ChannelKind.ReplyChannel replayTo, CancellationToken cancellation = default(CancellationToken));

        IDisposable ListenRequest<TCommand>(Expression<Func<T, ICall<TCommand>>> selector,
            Func<TCommand, RequestContext, CancellationToken, Task> listener, Action<ChannelBuilder> configure = null);

        IDisposable ListenRequest<TRequest, TResponse>(Expression<Func<T, ICall<TRequest, TResponse>>> selector,
            Func<TRequest, RequestContext<TResponse>, CancellationToken, Task> listener, Action<ChannelBuilder> configure = null);

        Task<TResponse> Call<TRequest, TResponse>(
            Expression<Func<T, ICall<TRequest, TResponse>>> selector,
            TRequest request, TimeSpan? timeout = null);

        Task Call<TCommand>(
            Expression<Func<T, ICall<TCommand>>> selector,
            TCommand request, TimeSpan? timeout = null);

        Task ResponseFault<TCommand>(Expression<Func<T, ICall<TCommand>>> selector,
            RequestFault fault, ChannelKind.ReplyChannel replayTo, CancellationToken cancellation = default(CancellationToken));

        Task<Guid> Send<TRequest, TResponse>(Expression<Func<T, ICall<TRequest, TResponse>>> selector, TRequest command,
            ChannelKind.RespondableChannel responseTo = null, CancellationToken cancellation = default(CancellationToken));

        Task ResponseFault<TRequest, TResponse>(Expression<Func<T, ICall<TRequest, TResponse>>> selector,
            RequestFault fault, ChannelKind.ReplyChannel replayTo, CancellationToken cancellation = default(CancellationToken));

        Task<Guid> DeliverFaultReply<TStore, TCommand>(IUnitOfWork<TStore> uow,
            Expression<Func<T, ICall<TCommand>>> selector, RequestFault fault,
            ChannelKind.ReplyChannel replayTo);

        Task<Guid> DeliverFaultReply<TStore, TRequest, TReplay>(IUnitOfWork<TStore> uow,
            Expression<Func<T, ICall<TRequest, TReplay>>> selector, RequestFault fault,
            ChannelKind.ReplyChannel replayTo);

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