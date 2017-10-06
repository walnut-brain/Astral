namespace Astral.Links
{
    public class Request<T>
    {
        public Request(string correlationId, string replyTo, T value)
        {
            CorrelationId = correlationId;
            ReplyTo = replyTo;
            Value = value;
        }

        public string CorrelationId { get; }
        public string ReplyTo { get; }
        public T Value { get; }
    }
}