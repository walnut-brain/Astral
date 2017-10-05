using System;
using RabbitLink.Services.Descriptions;

namespace RabbitLink.Services
{
    internal class CallEndpoint<TService, TArg, TResult> : ICallEndpoint<TService, TArg, TResult> 
        where TArg : class 
        where TResult : class
    {
        private CallDescription Description { get; }
        private ServiceLink Link { get; }

        public CallEndpoint(ServiceLink link, CallDescription description)
        {
            Description = description;
            Link = link;
        }

        public ILinkCallPublisher<TService, TArg, TResult> Publisher => new CallPublisher<TService, TArg, TResult>(Link, Description);
        public ILinkCallConsumer<TService, TArg, TResult> Consumer => new CallConsumer<TService, TArg, TResult>(Link, Description);
    }

    internal class CallEndpoint<TService, TArg> : ICallEndpoint<TService, TArg>
        where TArg : class
    {
        private CallDescription Description { get; }
        private ServiceLink Link { get; }
        
        public CallEndpoint(ServiceLink link, CallDescription description)
        {
            Description = description;
            Link = link;
        }

        public ILinkCallPublisher<TService, TArg> Publisher => new CallPublisher<TService, TArg>(Link, Description);
        public ILinkCallConsumer<TService, TArg> Consumer => new CallConsumer<TService, TArg>(Link, Description);
    }
}