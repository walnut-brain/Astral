using System;
using Microsoft.Extensions.Logging;

namespace Astral.Lavium
{
    public static class Lavium
    {
        public static volatile LogLevel LoggingLevel = LogLevel.Error;

        internal static ILogger Checked(this ILogger logger)
            => new FakeLogger(logger);

        private class FakeLogger : ILogger
        {
            private readonly ILogger _logger;

            public FakeLogger(ILogger logger)
            {
                _logger = logger;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                if (LoggingLevel <= logLevel)
                    _logger.Log(logLevel, eventId, state, exception, formatter);
            }

            public bool IsEnabled(LogLevel logLevel) => _logger.IsEnabled(logLevel);

            public IDisposable BeginScope<TState>(TState state) => _logger.BeginScope(state);
        }
    }
}