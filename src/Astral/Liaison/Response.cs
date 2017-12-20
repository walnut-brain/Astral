using System;

namespace Astral.Liaison
{
    /// <summary>
    /// Response envelope
    /// </summary>
    /// <typeparam name="T">response type</typeparam>
    public class Response<T>
    {
        /// <summary>
        /// error when processed
        /// </summary>
        /// <param name="error">error</param>
        /// <param name="correlationId">correlation id</param>
        /// <param name="replyTo">reply to hint</param>
        public Response(Exception error, string correlationId, string replyTo)
        {
            Error = error;
            CorrelationId = correlationId;
            ReplyTo = replyTo;
            Result = default(T);
        }

        /// <summary>
        /// success processed
        /// </summary>
        /// <param name="result">result</param>
        /// <param name="correlationId">correlation id</param>
        /// <param name="replyTo">reply to hint</param>
        public Response(T result, string correlationId, string replyTo)
        {
            Result = result;
            CorrelationId = correlationId;
            ReplyTo = replyTo;
            Error = null;
        }

        /// <summary>
        /// Correlation id
        /// </summary>
        public string CorrelationId { get; }
        /// <summary>
        /// reply to hint
        /// </summary>
        public string ReplyTo { get; }
        /// <summary>
        /// exception when error
        /// </summary>
        public Exception Error { get; }
        /// <summary>
        /// id error
        /// </summary>
        public bool IsFail => Error != null;
        /// <summary>
        /// result when success
        /// </summary>
        public T Result { get; }
    }
}