namespace Astral
{
    public class ResponseContext : ContextBase
    {
        public ResponseContext(string sender, string requestId) : base(sender)
        {
            RequestId = requestId;
        }
        
        public string RequestId { get; }
    }
}