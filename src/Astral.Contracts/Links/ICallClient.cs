using System;
using System.Threading;
using System.Threading.Tasks;

namespace Astral.Links
{
    /// <summary>
    /// call with result client
    /// </summary>
    /// <typeparam name="TArg">argument type</typeparam>
    /// <typeparam name="TResult">result type</typeparam>
    public interface ICallClient<TArg, TResult>
    {
        /// <summary>
        /// call server
        /// </summary>
        /// <param name="arg">argument</param>
        /// <param name="token">cancellation</param>
        /// <returns>awaitable result</returns>
        Task<TResult> Call(TArg arg, CancellationToken token = default(CancellationToken));
        
        /// <summary>
        /// call server
        /// </summary>
        /// <param name="arg">argument</param>
        /// <param name="timeout">timeout</param>
        /// <returns>awaiteble result</returns>
        Task<TResult> Call(TArg arg, TimeSpan timeout);
    }
    
    /// <summary>
    /// call with result client with service marker
    /// </summary>
    /// <typeparam name="TService">service type</typeparam>
    /// <typeparam name="TArg">argument type</typeparam>
    /// <typeparam name="TResult">result type</typeparam>
    public interface ICallClient<TService, TArg, TResult> : ICallClient<TArg, TResult>
    {
        
    }
}