using System;

namespace Astral.Logging
{
    public interface ILog
    {
        void Write(LogLevel level, string message, Exception ex = null);
        ILog With(string parameter, Func<object> value);
    }
}