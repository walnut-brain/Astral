using System;
using System.Threading;
using System.Threading.Tasks;
using Astral.Links;

namespace RabbitLink.Services
{
    /// <summary>
    /// Call action endpoint
    /// </summary>
    /// <typeparam name="TService">service type</typeparam>
    /// <typeparam name="TArg">call argument contract</typeparam>
    public interface ICallEndpoint<TService, TArg> : IActionPublisher<TService, TArg>, IActionConsumer<TService, TArg>
    {
        /// <summary>
        /// get queues prefetch count
        /// </summary>
        /// <returns>queues prefetch count</returns>
        ushort PrefetchCount();
        /// <summary>
        /// set queues prefetch count, default 1
        /// </summary>
        /// <param name="value">queues prefetch count value</param>
        /// <returns>call endpoint</returns>
        ICallEndpoint<TService, TArg> PrefetchCount(ushort value);
        
        /// <summary>
        /// get call timeout
        /// </summary>
        /// <returns>call timeout</returns>
        TimeSpan Timeout();
        /// <summary>
        /// set call timeout, default 10m
        /// </summary>
        /// <param name="value">call timeout</param>
        /// <returns>call endpoint</returns>
        ICallEndpoint<TService, TArg> Timeout(TimeSpan value);
        
        /// <summary>
        /// get response queue expires
        /// </summary>
        /// <returns>response queue expires</returns>
        TimeSpan? ResponseQueueExpires();
        /// <summary>
        /// set response queue expires, default null - never
        /// </summary>
        /// <param name="value">response queue expires value</param>
        /// <returns>call endpoint</returns>
        ICallEndpoint<TService, TArg> ResponseQueueExpires(TimeSpan? value);
        
    }

    /// <summary>
    /// Call function endpoint
    /// </summary>
    /// <typeparam name="TService">service type</typeparam>
    /// <typeparam name="TArg">call argument contract</typeparam>
    /// <typeparam name="TResult">call result contract</typeparam>
    public interface ICallEndpoint<TService, TArg, TResult> : ICallPublisher<TService, TArg, TResult>, ICallConsumer<TService, TArg, TResult>
    {
        /// <summary>
        /// get queues prefetch count
        /// </summary>
        /// <returns>queues prefetch count</returns>
        ushort PrefetchCount();
        /// <summary>
        /// set queues prefetch count, default 1
        /// </summary>
        /// <param name="value">queues prefetch count value</param>
        /// <returns>call endpoint</returns>
        ICallEndpoint<TService, TArg, TResult> PrefetchCount(ushort value);
        
        /// <summary>
        /// get call timeout
        /// </summary>
        /// <returns>call timeout</returns>
        TimeSpan Timeout();
        /// <summary>
        /// set call timeout, default 10m
        /// </summary>
        /// <param name="value">call timeout</param>
        /// <returns>call endpoint</returns>
        ICallEndpoint<TService, TArg, TResult> Timeout(TimeSpan value);
        
        /// <summary>
        /// get response queue expires
        /// </summary>
        /// <returns>response queue expires</returns>
        TimeSpan? ResponseQueueExpires();
        /// <summary>
        /// set response queue expires, default null - never
        /// </summary>
        /// <param name="value">response queue expires value</param>
        /// <returns>call endpoint</returns>
        ICallEndpoint<TService, TArg, TResult> ResponseQueueExpires(TimeSpan? value);
        
    }
}