using System;

namespace Astral.Logging
{
    public class NullLogFactory : ILogFactory 
    {
        public ILog CreateLog(string category) => new NullLog();

        public ILog CreateLog<T>() => new NullLog();

        public ILog CreateLog(Type type) => new NullLog();

        private class NullLog : ILog
        {
            public void Write(LogLevel level, string message, Exception ex = null)
            {
                
            }

            public ILog With(string parameter, Func<object> value) => this;
        }
    }
}