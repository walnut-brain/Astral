namespace Astral
{
    public class RequestContext : ContextBase
    {
        public RequestContext(string sender, ChannelKind.ReplyChannel replyTo, IResponder response) : base(sender)
        {
            ReplyTo = replyTo;
            Response = response;
        }

        public ChannelKind.ReplyChannel ReplyTo { get; }
        
        public IResponder Response { get; }
    }
    
    public class RequestContext<TResponse> : ContextBase
    {
        public RequestContext(string sender, ChannelKind.ReplyChannel replyTo, IResponder<TResponse> response) : base(sender)
        {
            ReplyTo = replyTo;
            Response = response;
        }

        public ChannelKind.ReplyChannel ReplyTo { get; }
        
        public IResponder<TResponse> Response { get; }
    }
}