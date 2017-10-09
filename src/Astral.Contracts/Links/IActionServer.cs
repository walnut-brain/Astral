using System;
using System.Threading;
using System.Threading.Tasks;

namespace Astral.Links
{
    /// <summary>
    /// call without result server
    /// </summary>
    /// <typeparam name="TArg">argument type</typeparam>
    public interface IActionServer<TArg>
    {
        /// <summary>
        /// set request processing handler 
        /// </summary>
        /// <param name="processor">processor handler</param>
        /// <returns>dispose unprocess</returns>
        IDisposable Process(Func<TArg, CancellationToken, Task> processor);
    }

    /// <summary>
    /// call without result server with service marker
    /// </summary>
    /// <typeparam name="TService">service type</typeparam>
    /// <typeparam name="TArg">argument type</typeparam>
    public interface IActionServer<TService, TArg> : IActionServer<TArg>
    {
        
    }
}