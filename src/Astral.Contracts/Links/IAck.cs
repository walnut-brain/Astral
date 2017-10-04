using System;

namespace Astral.Links
{
    public interface IAck<out T>
    {
        T Value { get; }
        Action<Acknowledge> SetResult { get; }
        Action<Exception> SetError { get; }
        
    }
}