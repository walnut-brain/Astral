namespace Astral.Liaison
{
    /// <summary>
    /// Acknowledge
    /// </summary>
    public enum Acknowledge
    {
        /// <summary>
        /// Accepted
        /// </summary>
        Ack,
        /// <summary>
        /// Not accaepted
        /// </summary>
        Nack,
        /// <summary>
        /// Repeat later
        /// </summary>
        Requeue
    }
}