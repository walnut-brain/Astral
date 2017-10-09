using System;
using Microsoft.Extensions.Logging;

namespace RabbitLink.Services.Logging
{
    internal class FakeLoggerFactory : ILoggerFactory
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
            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                
            }

            public bool IsEnabled(LogLevel logLevel) => false;
            

            public IDisposable BeginScope<TState>(TState state) => new DelegateDisposable(() => {});
        }
    }
}