using System;
using Astral.Logging;
using Microsoft.Extensions.Logging;
using RabbitLink.Logging;

namespace Astral.RabbitLink.Logging
{
    internal class LoggerAdapter : ILinkLogger
    {
        private readonly ILog _logger;

        public LoggerAdapter(ILog logger)
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
                    _logger.Error(message);
                    break;
                case LinkLoggerLevel.Warning:
                    _logger.Warn(message);
                    break;
                case LinkLoggerLevel.Info:
                    _logger.Info(message);
                    break;
                case LinkLoggerLevel.Debug:
                    _logger.Debug(message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }
    }
}