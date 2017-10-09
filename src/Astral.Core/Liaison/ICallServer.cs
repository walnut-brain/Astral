using System;
using System.Threading;
using System.Threading.Tasks;

namespace Astral.Liaison
{
    /// <summary>
    /// call with result server
    /// </summary>
    /// <typeparam name="TArg">argument type</typeparam>
    /// <typeparam name="TResult">result type</typeparam>
    public interface ICallServer<TArg, TResult>
    {
        /// <summary>
        /// set process handler
        /// </summary>
        /// <param name="processor">process handler</param>
        /// <returns>disposable shutdown</returns>
        IDisposable Process(Func<TArg, CancellationToken, Task<TResult>> processor);
    }

    /// <summary>
    /// call with result server with service marker
    /// </summary>
    /// <typeparam name="TService">service type</typeparam>
    /// <typeparam name="TArg">argument type</typeparam>
    /// <typeparam name="TResult">result type</typeparam>
    public interface ICallServer<TService, TArg, TResult> : ICallServer<TArg, TResult>
    {
        
    }
}