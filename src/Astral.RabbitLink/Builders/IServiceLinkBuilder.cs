using System;
using Astral.Logging;
using Astral.RabbitLink.Descriptions;
using Microsoft.Extensions.Logging;
using RabbitLink;
using RabbitLink.Connection;
using RabbitLink.Serialization;

namespace Astral.RabbitLink
{
    /// <summary>
    /// service link builder
    /// </summary>
    public interface IServiceLinkBuilder 
    {
        
        /// <summary>
        /// get connection name
        /// </summary>
        /// <returns>connection name</returns>
        string ConnectionName();
        
        /// <summary>
        /// set connection name
        /// </summary>
        /// <param name="value">connection name value</param>
        /// <returns>service link builder</returns>
        IServiceLinkBuilder ConnectionName(string value);
        
        /// <summary>
        /// set uri of RabbitMq server
        /// </summary>
        /// <param name="value">uri of RabbitMq server value</param>
        /// <returns>service link builder</returns>
        IServiceLinkBuilder Uri(string value);
        
        /// <summary>
        /// get RabbitMq server uri
        /// </summary>
        /// <returns>RabbitMq server uri</returns>
        Uri Uri();
        /// <summary>
        /// set uri of RabbitMq server
        /// </summary>
        /// <param name="value">uri of RabbitMq server value</param>
        /// <returns>service link builder</returns>
        IServiceLinkBuilder Uri(Uri value);
        
        
        /// <summary>
        /// get autostart service link 
        /// </summary>
        /// <returns>autostart service link</returns>
        bool AutoStart();
        
        /// <summary>
        /// set autostart service link, default true 
        /// </summary>
        /// <param name="value">autostart service link</param>
        /// <returns>service link builder</returns>
        IServiceLinkBuilder AutoStart(bool value);
        
        
        /// <summary>
        /// get connection timeout
        /// </summary>
        /// <returns></returns>
        TimeSpan Timeout();
        /// <summary>
        /// set connection timeout, default 10s 
        /// </summary>
        /// <param name="value">connection timeout value</param>
        /// <returns>service link builder</returns>
        IServiceLinkBuilder Timeout(TimeSpan value);
        
        
        /// <summary>
        /// get connection recovery interval
        /// </summary>
        /// <returns>connection recovery interval</returns>
        TimeSpan RecoveryInterval();
        /// <summary>
        /// set connection recovery interval, default 10s 
        /// </summary>
        /// <param name="value">connection recovery interval value</param>
        /// <returns>service link builder</returns>
        IServiceLinkBuilder RecoveryInterval(TimeSpan value);
        
        /// <summary>
        /// get link holder name
        /// </summary>
        /// <returns>link holder name</returns>
        string HolderName();
        /// <summary>
        /// set link holder name  
        /// </summary>
        /// <param name="value">link holder name</param>
        /// <returns>service link builder</returns>
        IServiceLinkBuilder HolderName(string value);
        
        /// <summary>
        /// get on connection state changed delegate
        /// </summary>
        /// <returns>connection state changed delegate</returns>
        LinkStateHandler<LinkConnectionState> OnStateChange();
        /// <summary>
        /// set connection state changed delegate  
        /// </summary>
        /// <param name="handler">connection state changed delegate</param>
        /// <returns>service link builder</returns>
        IServiceLinkBuilder OnStateChange(LinkStateHandler<LinkConnectionState> handler);
        
        /// <summary>
        /// get use background threads for connection
        /// </summary>
        /// <returns>use background threads for connection</returns>
        bool UseBackgroundThreadsForConnection();
        /// <summary>
        /// set use background threads for connection, default false  
        /// </summary>
        /// <param name="value">use background threads for connection value</param>
        /// <returns>service link builder</returns>
        IServiceLinkBuilder UseBackgroundThreadsForConnection(bool value);
        
        
        /// <summary>
        /// get link payload manager
        /// </summary>
        /// <returns>link payload manager</returns>
        IPayloadManager PayloadManager();
        /// <summary>
        /// set link payload manager
        /// </summary>
        /// <param name="value">link payload manager value</param>
        /// <returns>service link builder</returns>
        IServiceLinkBuilder PayloadManager(IPayloadManager value);
        
        /// <summary>
        /// get description factory
        /// </summary>
        /// <returns>description factory</returns>
        IDescriptionFactory DescriptionFactory();
        
        /// <summary>
        /// set description factory
        /// </summary>
        /// <param name="value">description factory value</param>
        /// <returns>service link builder</returns>
        IServiceLinkBuilder DescriptionFactory(IDescriptionFactory value);
        
        /// <summary>
        /// get log factory
        /// </summary>
        /// <returns>log factory</returns>
        ILogFactory LogFactory();
        
        /// <summary>
        /// set logger factory 
        /// </summary>
        /// <param name="value">logger factory value</param>
        /// <returns>service link builder</returns>
        IServiceLinkBuilder LoggerFactory(ILoggerFactory value);

        /// <summary>
        /// set log factory
        /// </summary>
        /// <param name="value">log factory value</param>
        /// <returns>service link builder</returns>
        IServiceLinkBuilder LogFactory(ILogFactory value);

        /// <summary>
        /// build service link 
        /// </summary>
        /// <returns>service link</returns>
        IServiceLink Build();


        IServiceLinkBuilder Serializer(ILinkSerializer serializer);
    }
}