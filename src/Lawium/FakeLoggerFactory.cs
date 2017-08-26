using Microsoft.Extensions.Logging;

namespace Lawium
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