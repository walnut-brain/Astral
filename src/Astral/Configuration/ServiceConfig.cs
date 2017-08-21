using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using Astral.Configuration.Settings;
using Astral.Core;
using Microsoft.Extensions.Logging;
using Astral.Lavium;

namespace Astral.Configuration
{
    public class ServiceConfig<T> : ConfigBase, IServiceConfig
    {
        private readonly ILogger<ServiceConfig<T>> _logger; 
        private readonly ConcurrentDictionary<string, EndpointConfig> _endpoints = new ConcurrentDictionary<string, EndpointConfig>();
        
        internal ServiceConfig(LawBook config) : base(config)
        {
            _logger = GetLogger<ServiceConfig<T>>();
            Config.RegisterAxiom(new ServiceType(typeof(T)));
        }

        
        
        public EndpointConfig Endpoint<TContract>(Expression<Func<T, IEvent<TContract>>> selector)
        {
            using (_logger.BeginScope(("Endpoint requested", selector)))
            {
                var propInfo = selector.GetProperty();

                return _endpoints.GetOrAdd(propInfo.Name, _ =>
                    {
                        var book = Config.GetOrAddBook(propInfo.Name);
                        book.RegisterAxiom(propInfo);
                        book.RegisterAxiom(EndpointType.Event);
                        book.RegisterAxiom(new MessageType(typeof(TContract)));
                        return new EndpointConfig(book);
                    })
                    .WithEffect(() => _logger.LogTrace("Created"));
            }
        }
    }
}