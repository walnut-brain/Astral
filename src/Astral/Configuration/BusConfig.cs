using System;
using System.Collections.Concurrent;
using System.Reflection;
using Astral.Configuration.Settings;
using Astral.Lavium;
using Microsoft.Extensions.Logging;


namespace Astral.Configuration
{
    public class BusConfig : ConfigBase
    {
        
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<BusConfig> _logger;
        private readonly ConcurrentDictionary<Type, IServiceConfig> _serviceConfigs = new ConcurrentDictionary<Type, IServiceConfig>();  

        public BusConfig(SystemName systemName, ILoggerFactory loggerFactory)
            : base(new LawBook(loggerFactory.CreateLogger<LawBook>()))
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = loggerFactory.CreateLogger<BusConfig>();
            Config.RegisterAxiom(loggerFactory);
            Config.RegisterAxiom(systemName);
            Config.RegisterAxiom(new InstanceCode(Environment.MachineName+ "." + Guid.NewGuid().ToString("D")));
        }

        

        public ServiceConfig<T> Service<T>()
        {
            var type = typeof(T);
            using (_logger.BeginScope(("Service requested", type)))
            {
                var typeInfo = type.GetTypeInfo();
                if (!typeInfo.IsInterface)
                    throw new ArgumentException($"{type} must be interface");
                return (ServiceConfig<T>) _serviceConfigs.GetOrAdd(type, _ => new ServiceConfig<T>(Config.GetOrAddBook(type)))
                    .WithEffect(() => _logger.LogTrace("Created"));
            }
        }
        
        
    }
}