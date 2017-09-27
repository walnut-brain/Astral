using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Astral.Configuration.Builders;
using Astral.Data;
using Astral.Deliveries;

namespace Astral
{
    public interface IEventEndpoint<TEvent>
    {
        /// <summary>
        ///     Publish event asynchronius
        /// </summary>
        /// <param name="event">published event</param>
        /// <param name="token">cancellation</param>
        /// <typeparam name="TEvent">event type</typeparam>
        /// <returns>awaitable task</returns>
        Task PublishAsync(TEvent @event, CancellationToken token = default(CancellationToken));


        /// <summary>
        ///     Listen event
        /// </summary>
        /// <param name="eventListener">event listener</param>
        /// <param name="channel">channel type, default System</param>
        /// <param name="configure">configure channel, default from config</param>
        /// <typeparam name="TEvent">event type</typeparam>
        /// <returns>dispose to unlisten</returns>
        IDisposable Listen<TChannel>(Func<TEvent, EventContext, CancellationToken, Task> eventListener, TChannel channel = null, Action<ChannelBuilder> configure = null)
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
        Task<Guid> Deliver<TStore>(IUnitOfWork<TStore> uow, TEvent @event, DeliveryOnSuccess? onSuccess = null);
        
        /// <summary>
        /// Enqueue event delivery
        /// </summary>
        /// <param name="uow">current unit of work</param>
        /// <param name="event">event</param>
        /// <typeparam name="TStore">store type</typeparam>
        /// <returns>awaitable delivery uid</returns>
        Task<Guid> Enqueue<TStore>(IUnitOfWork<TStore> uow, TEvent @event);
    }
}