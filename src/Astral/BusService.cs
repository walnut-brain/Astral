using System;
using Astral.Configuration.Configs;
using Astral.Transport;
using Microsoft.Extensions.Logging;

namespace Astral
{
    public class BusService<TTransport, TService>
        where TTransport : class, ITransport
        where TService : class

    {
        internal BusService(ServiceConfig<TService> config, TTransport transport, ILogger logger, IServiceProvider provider)
        {
            Config = config;
            Transport = transport;
            Logger = logger;
            Provider = provider;
        }

        internal ServiceConfig<TService> Config { get; }
        internal TTransport Transport { get; }
        internal ILogger Logger { get; }
        internal IServiceProvider Provider { get; }
    }
}