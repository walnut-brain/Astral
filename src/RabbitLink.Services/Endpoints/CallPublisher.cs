using RabbitLink.Services.Descriptions;

namespace RabbitLink.Services
{
    internal class CallPublisher<TService, TArg, TResult> : ILinkCallPublisher<TService, TArg, TResult> 
        where TArg : class 
        where TResult : class
    {
        protected ServiceLink Link { get; }
        protected CallDescription Description { get; }

        public CallPublisher(ServiceLink link, CallDescription description)
        {
            Link = link;
            Description = description;
        }
    }
    
    internal class CallPublisher<TService, TArg> : CallPublisher<TService, TArg, Unit>, ILinkCallPublisher<TService, TArg>
        where TArg : class 
    {
        public CallPublisher(ServiceLink link, CallDescription description) : base(link, description)
        {
        }
    }
}