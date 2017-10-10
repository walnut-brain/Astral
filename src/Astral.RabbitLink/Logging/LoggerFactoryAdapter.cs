using Astral.Logging;
using Microsoft.Extensions.Logging;
using RabbitLink.Logging;

namespace Astral.RabbitLink.Logging
{
    internal class LoggerFactoryAdapter : ILinkLoggerFactory
    {
        private readonly ILogFactory _factory;

        public LoggerFactoryAdapter(ILogFactory factory)
        {
            _factory = factory;
        }

        public ILinkLogger CreateLogger(string name)
            => new LoggerAdapter(_factory.CreateLog(name));
    }
}