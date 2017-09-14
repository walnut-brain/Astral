using System;
using Microsoft.Extensions.Logging;
using RabbitLink.Logging;

namespace RabbitLink.Astral
{
    internal class LinkLoggerFactory : ILinkLoggerFactory
    {
        private readonly ILoggerFactory _factory;

        public LinkLoggerFactory(ILoggerFactory factory)
        {
            _factory = factory;
        }

        public ILinkLogger CreateLogger(string name)
            => new LinkLogger(_factory.CreateLogger(name));


        private class LinkLogger : ILinkLogger
        {
            private readonly ILogger _logger;

            public LinkLogger(ILogger logger)
            {
                _logger = logger;
            }

            public void Dispose()
            {
                
            }

            public void Write(LinkLoggerLevel level, string message)
            {
                switch (level)
                {
                    case LinkLoggerLevel.Error:
                        _logger.LogError(message);
                        break;
                    case LinkLoggerLevel.Warning:
                        _logger.LogWarning(message);
                        break;
                    case LinkLoggerLevel.Info:
                        _logger.LogInformation(message);
                        break;
                    case LinkLoggerLevel.Debug:
                        _logger.LogDebug(message);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(level), level, null);
                }
            }
        }
    }
}