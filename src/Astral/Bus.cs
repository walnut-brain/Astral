using System;
using Astral.Configuration;
using Astral.Configuration.Configs;

namespace Astral
{
    public class Bus : IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly BusConfig _config;

        public Bus(IServiceProvider serviceProvider, BusConfig config)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        



        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}