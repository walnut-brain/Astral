using System;
using System.Reactive.Disposables;
using Microsoft.Extensions.Logging;

namespace Astral.Logging
{
    public class FakeLoggerFactory : ILoggerFactory
    {
        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public ILogger CreateLogger(string categoryName)
            => new FakeLogger();

        public void AddProvider(ILoggerProvider provider)
        {
            
        }
        
        private class FakeLogger : ILogger
        {
            public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                
            }

            public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel) => false;
            

            public IDisposable BeginScope<TState>(TState state) => Disposable.Empty;
        }
    }
}