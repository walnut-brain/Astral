using System;

namespace Astral.Links
{
    public class Response<T>
    {
        public Response(Exception error, string correlationId, string replyTo)
        {
            Error = error;
            CorrelationId = correlationId;
            ReplyTo = replyTo;
            Result = default(T);
        }

        public Response(T result, string correlationId, string replyTo)
        {
            Result = result;
            CorrelationId = correlationId;
            ReplyTo = replyTo;
            Error = null;
        }

        public string CorrelationId { get; }
        public string ReplyTo { get; }
        public Exception Error { get; }
        public bool IsFail => Error != null;
        public T Result { get; }
    }
}