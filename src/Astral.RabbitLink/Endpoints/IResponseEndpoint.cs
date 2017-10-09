using Astral.Liaison;

namespace Astral.RabbitLink
{
    /// <summary>
    /// response endpoint
    /// </summary>
    /// <typeparam name="TService">service type</typeparam>
    /// <typeparam name="TRequest">request type</typeparam>
    /// <typeparam name="TResponse">response type</typeparam>
    public interface IResponseEndpoint<TService, TRequest, TResponse> : IPublisher<TService, Response<TResponse>>, IConsumer<TService, Request<TRequest>>
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
        IResponseEndpoint<TService, TRequest, TResponse> PrefetchCount(ushort value);
        
        /// <summary>
        /// get response producer confirms mode
        /// </summary>
        /// <returns>response producer confirms mode</returns>
        bool ConfirmsMode();
        
        /// <summary>
        /// set response producer confirms mode
        /// </summary>
        /// <param name="value">response producer confirms mode value</param>
        /// <returns>call endpoint</returns>
        IResponseEndpoint<TService, TResponse, TRequest> ConfirmsMode(bool value);
    }
}