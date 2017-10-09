namespace Astral.Links
{
    /// <summary>
    /// request envelope
    /// </summary>
    /// <typeparam name="T">message type</typeparam>
    public class Request<T>
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="correlationId">message correlation id</param>
        /// <param name="replyTo">message replay to hint</param>
        /// <param name="value">message</param>
        public Request(string correlationId, string replyTo, T value)
        {
            CorrelationId = correlationId;
            ReplyTo = replyTo;
            Value = value;
        }

        /// <summary>
        /// correlation id
        /// </summary>
        public string CorrelationId { get; }
        /// <summary>
        /// replay to hint
        /// </summary>
        public string ReplyTo { get; }
        /// <summary>
        /// message
        /// </summary>
        public T Value { get; }
    }
}