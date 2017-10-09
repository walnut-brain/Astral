using System;
using Microsoft.Extensions.Logging;

namespace Astral.RabbitLink.Logging
{
    public static class LoggerExtensions
    {
        private class WithLogger : ILogger
        {
            private readonly ILogger _logger;
            private readonly string _fmt;
            private readonly object[] _values;

            public WithLogger(ILogger logger, string fmt, params object[] values)
            {
                _logger = logger;
                _fmt = fmt;
                _values = values;
            }


            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
                Func<TState, Exception, string> formatter)
            {
                using(_logger.BeginScope(_fmt, _values))
                    _logger.Log(logLevel, eventId, state, exception, formatter);
            } 

            public bool IsEnabled(LogLevel logLevel)
                => _logger.IsEnabled(logLevel);

            public IDisposable BeginScope<TState>(TState state)
                => _logger.BeginScope(state);
        }
        
        public static ILogger With(this ILogger logger, string fmt, params object[] args)
            => new WithLogger(logger, fmt, args);
    }
}