using System;
using System.Threading;
using System.Threading.Tasks;

namespace Astral.Liaison
{
    /// <summary>
    /// consumer 
    /// </summary>
    /// <typeparam name="TMessage">message type</typeparam>
    public interface IConsumer<out TMessage> 
    {
        /// <summary>
        /// subsribe to message
        /// </summary>
        /// <param name="listener">subscriber</param>
        /// <returns>dispose to unsubscribe</returns>
        IDisposable Listen(Func<TMessage, CancellationToken, Task<Acknowledge>> listener);
    }
    
    /// <summary>
    /// consumer with service marker
    /// </summary>
    /// <typeparam name="TService">service type</typeparam>
    /// <typeparam name="TMessage">message type</typeparam>
    public interface IConsumer<TService, out TMessage> : IConsumer<TMessage>
    {
        
    }
}