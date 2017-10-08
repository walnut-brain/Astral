using System;

namespace Astral.Links
{
    /// <summary>
    /// Message with acknowledge
    /// </summary>
    /// <typeparam name="T">message type</typeparam>
    public interface IAck<out T>
    {
        /// <summary>
        /// message value
        /// </summary>
        T Value { get; }
        /// <summary>
        /// set acknowledge
        /// </summary>
        Action<Acknowledge> SetResult { get; }
        /// <summary>
        /// set exception - acknowledge from exception
        /// </summary>
        Action<Exception> SetError { get; }
        
    }
}