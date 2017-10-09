using System;
using System.Net.Mime;
using Astral.Links;
using RabbitLink.Consumer;

namespace RabbitLink.Services
{
    /// <summary>
    /// request endpoint
    /// </summary>
    /// <typeparam name="TService">service type</typeparam>
    /// <typeparam name="TRequest">request type</typeparam>
    /// <typeparam name="TResponse">response type</typeparam>
    public interface IRequestEndpoint<TService, TRequest, TResponse> : IConsumer<TService, Response<TResponse>>, IPublisher<TService, Request<TRequest>>
    {
        /// <summary>
        /// Content type
        /// </summary>
        ContentType ContentType { get; }
        
        /// <summary>
        /// get declare exchange passive
        /// </summary>
        /// <returns>declare exchange passive</returns>
        bool ExchangePassive();
        /// <summary>
        /// set declare exchange passive, default false
        /// </summary>
        /// <param name="value">declare exchange passive value</param>
        /// <returns>event endpoint</returns>
        IRequestEndpoint<TService, TResponse, TRequest> ExchangePassive(bool value);
        
        /// <summary>
        /// get use named producer
        /// </summary>
        /// <returns>use named producer</returns>
        string NamedProducer();
        /// <summary>
        /// set use named producer, default null - not use
        /// </summary>
        /// <param name="value">use named producer value</param>
        /// <returns>event endpoint</returns>
        IRequestEndpoint<TService, TResponse, TRequest> NamedProducer(string value);
        
        /// <summary>
        /// get producer confirms mode
        /// </summary>
        /// <returns>producer confirms mode</returns>
        bool ConfirmsMode();
        // <summary>
        /// set producer confirms mode, default true
        /// </summary>
        /// <param name="value">producer confirms mode value</param>
        /// <returns>event endpoint</returns>
        IRequestEndpoint<TService, TResponse, TRequest> ConfirmsMode(bool value);
        
        /// <summary>
        /// get queue name 
        /// </summary>
        /// <returns>queue name</returns>
        string QueueName();
        
        /// <summary>
        /// set queue name, default {Holder}.{Owner}.{Service}.{Endpoint}
        /// </summary>
        /// <param name="value">queue name value</param>
        /// <returns>event endpoint</returns>
        IRequestEndpoint<TService, TResponse, TRequest> QueueName(string value);
        
        /// <summary>
        /// get queue prefetch count
        /// </summary>
        /// <returns>queue prefetch count</returns>
        ushort PrefetchCount();
        
        /// <summary>
        /// set queue prefetch count, default 1
        /// </summary>
        /// <param name="value">queue prefetch count value</param>
        /// <returns>event endpoint</returns>
        IRequestEndpoint<TService, TResponse, TRequest> PrefetchCount(ushort value);
        
        /// <summary>
        /// get queue auto ack
        /// </summary>
        /// <returns>queue auto ack</returns>
        bool AutoAck();
        
        /// <summary>
        /// set queue auto ack, default false
        /// </summary>
        /// <param name="value">queue auto ack value</param>
        /// <returns>event endpoint</returns>
        IRequestEndpoint<TService, TResponse, TRequest> AutoAck(bool value);
        
        /// <summary>
        /// get consumer error strategy
        /// </summary>
        /// <returns>consumer error strategy</returns>
        ILinkConsumerErrorStrategy ErrorStrategy();
        
        /// <summary>
        /// set consumer error strategy, default from RabbitLink
        /// </summary>
        /// <param name="value">consumer error strategy value</param>
        /// <returns>event endpoint</returns>
        IRequestEndpoint<TService, TResponse, TRequest> ErrorStrategy(ILinkConsumerErrorStrategy value);
        
        /// <summary>
        /// get Cancel On Ha Failover
        /// </summary>
        /// <returns>Cancel On Ha Failover</returns>
        bool? CancelOnHaFailover();
        
        /// <summary>
        /// set CancelOnHaFailover, default null
        /// </summary>
        /// <param name="value">CancelOnHaFailover value</param>
        /// <returns>event endpoint</returns>
        IRequestEndpoint<TService, TResponse, TRequest> CancelOnHaFailover(bool? value);
        
        /// <summary>
        /// get queue exclusive 
        /// </summary>
        /// <returns>queue exclusive</returns>
        bool Exclusive();
        
        /// <summary>
        /// set queue exclusive, default false
        /// </summary>
        /// <param name="value">queue exclusive value</param>
        /// <returns>event endpoint</returns>
        IRequestEndpoint<TService, TResponse, TRequest> Exclusive(bool value);
        
        /// <summary>
        /// get declare queue passive
        /// </summary>
        /// <returns>declare queue passive</returns>
        bool QueuePassive();
        
        /// <summary>
        /// set declare queue passive, default false
        /// </summary>
        /// <param name="value">declare queue passive value</param>
        /// <returns>event endpoint</returns>
        IRequestEndpoint<TService, TResponse, TRequest> QueuePassive(bool value);
        
        /// <summary>
        /// get bind queue to exchange
        /// </summary>
        /// <returns>bind queue to exchange</returns>
        bool Bind();
        
        /// <summary>
        /// set bind queue to exchange, default true
        /// </summary>
        /// <param name="value">bind queue to exchange value</param>
        /// <returns>event endpoint</returns>
        IRequestEndpoint<TService, TResponse, TRequest> Bind(bool value);
        
        /// <summary>
        /// get queue parameters
        /// </summary>
        /// <returns>queue parameters</returns>
        QueueParameters QueueParameters();
        
        /// <summary>
        /// set queue parameters
        /// </summary>
        /// <param name="setter">queue parameters changer</param>
        /// <returns>event endpoint</returns>
        IRequestEndpoint<TService, TResponse, TRequest> QueueParameters(Func<QueueParameters, QueueParameters> setter);
    }
}