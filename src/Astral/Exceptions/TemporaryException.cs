using System;

namespace Astral
{
    public class TemporaryException : Exception
    {
        public TemporaryException(TimeSpan pause)
        {
            if (pause < TimeSpan.Zero) pause = TimeSpan.Zero;
            Pause = pause;
        }

        public TemporaryException(TimeSpan pause, string message) : base(message)
        {
            if (pause < TimeSpan.Zero) pause = TimeSpan.Zero;
            Pause = pause;
        }

        public TemporaryException(TimeSpan pause, string message, Exception innerException) : base(message,
            innerException)
        {
            if (pause < TimeSpan.Zero) pause = TimeSpan.Zero;
            Pause = pause;
        }

        public TimeSpan Pause { get; }
    }
}