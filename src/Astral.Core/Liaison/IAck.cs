using System;

namespace Astral.Liaison
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