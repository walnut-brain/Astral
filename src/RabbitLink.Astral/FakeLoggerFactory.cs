using Microsoft.Extensions.Logging;

namespace RabbitLink.Astral
{
    internal class FakeLoggerFactory : ILoggerFactory
    {
        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new FakeLogger();
        }

        public void AddProvider(ILoggerProvider provider)
        {
            throw new System.NotSupportedException();
        }
    }
}