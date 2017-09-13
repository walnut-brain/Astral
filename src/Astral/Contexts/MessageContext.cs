namespace Astral
{
    public class MessageContext
    {
        public MessageContext(string sender, string requestId, string replayTo)
        {
            Sender = sender;
            RequestId = requestId;
            ReplayTo = replayTo;
        }

        public string Sender { get; }
        public string RequestId { get; }
        public string ReplayTo { get; }
    }
}