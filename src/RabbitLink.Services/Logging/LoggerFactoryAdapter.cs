using Microsoft.Extensions.Logging;
using RabbitLink.Logging;

namespace RabbitLink.Services.Logging
{
    internal class LoggerFactoryAdapter : ILinkLoggerFactory
    {
        private readonly ILoggerFactory _factory;

        public LoggerFactoryAdapter(ILoggerFactory factory)
        {
            _factory = factory;
        }

        public ILinkLogger CreateLogger(string name)
            => new LoggerAdapter(_factory.CreateLogger(name));
    }
}