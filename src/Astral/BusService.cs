using System;
using Astral.Configuration.Configs;
using Astral.Transport;
using Microsoft.Extensions.Logging;

namespace Astral
{
    public class BusService<TService> : IBusService<TService>
        where TService : class

    {
        private readonly ILoggerFactory _loggerFactory;

        internal BusService(ServiceConfig<TService> config, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            Config = config;
            Logger = _loggerFactory.CreateLogger<BusService<TService>>();
        }

        internal ServiceConfig<TService> Config { get; }
        
        public ILogger Logger { get; }
    }
}