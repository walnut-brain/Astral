using Microsoft.Extensions.Logging;

namespace Astral.Fakes
{
    public class FakeLoggerFactory : ILoggerFactory
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