using System;
using System.Threading;

namespace Astral.Links
{
    public class Ack<T> : IAck<T>
    {
        public Ack(T value, Action<Acknowledge> setAcknowledge, Action<Exception> setError)
        {
            Value = value;
            SetResult = setAcknowledge;
            SetError = setError;
        }

        public T Value { get; }
        public Action<Acknowledge> SetResult { get; }
        public Action<Exception> SetError { get; }
    }
}