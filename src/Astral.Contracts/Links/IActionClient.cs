using System;
using System.Threading;
using System.Threading.Tasks;

namespace Astral.Links
{
    /// <summary>
    /// call without result client
    /// </summary>
    /// <typeparam name="TArg">argument type</typeparam>
    public interface IActionClient<TArg>
    {
        /// <summary>
        /// call server
        /// </summary>
        /// <param name="arg">arguments</param>
        /// <param name="token">cancellation</param>
        /// <returns>awaiteable processed</returns>
        Task Call(TArg arg, CancellationToken token = default (CancellationToken));
        
        /// <summary>
        /// call server
        /// </summary>
        /// <param name="arg">arguments</param>
        /// <param name="timeout">timeout</param>
        /// <returns>awaitable processed</returns>
        Task Call(TArg arg, TimeSpan timeout);
    }

    /// <summary>
    /// call without result client with service marker
    /// </summary>
    /// <typeparam name="TService">service type</typeparam>
    /// <typeparam name="TArg">argument type</typeparam>
    public interface IActionClient<TService, TArg> : IActionClient<TArg>
    {
    }
}