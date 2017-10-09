using System.Threading;
using System.Threading.Tasks;

namespace Astral.Liaison
{
    /// <summary>
    /// publisher
    /// </summary>
    /// <typeparam name="TMessage">message type</typeparam>
    public interface IPublisher<in TMessage> 
    {
        /// <summary>
        /// send message async
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="token">cancellation</param>
        /// <returns>awaiteble sended</returns>
        Task PublishAsync(TMessage message, CancellationToken token = default(CancellationToken));
    }

    /// <summary>
    /// publisher with service marker
    /// </summary>
    /// <typeparam name="TService">service type</typeparam>
    /// <typeparam name="TMessage">message type</typeparam>
    public interface IPublisher<TService, in TMessage> : IPublisher<TMessage>
    {
        
    }
}