using System;
using Astral.Transport;
using Lawium;
using Microsoft.Extensions.Logging;

namespace Astral.Configuration.Builders
{
    public class CallEndpointBuilderBase<TArgs, TResult> : EndpointBuilder
    {
        public CallEndpointBuilderBase(LawBookBuilder<Fact> bookBuilder) : base(bookBuilder)
        {
        }
        
        public ChannelBuilder SystemChannel => ChannelBuilder(SubscribeChannel.System, false);
        public ChannelBuilder InstanceChannel => ChannelBuilder(SubscribeChannel.Instance, false);
        public ChannelBuilder DedicatedChannel => ChannelBuilder(SubscribeChannel.Dedicated, false);
        public ChannelBuilder NamedBuilder(string name = "<<default>>") => ChannelBuilder(SubscribeChannel.Named(name), false);
        
        
        public ChannelBuilder ResponseSystemChannel => ChannelBuilder(SubscribeChannel.System, true);
        public ChannelBuilder ResponseInstanceChannel => ChannelBuilder(SubscribeChannel.Instance, true);
        public ChannelBuilder ResponseDedicatedChannel => ChannelBuilder(SubscribeChannel.Dedicated, true);
        public ChannelBuilder ResponseNamedBuilder(string name = "<<default>>") => ChannelBuilder(SubscribeChannel.Named(name), true);
        public ChannelBuilder ResponseRpcChannel => ChannelBuilder(SubscribeChannel.Rpc, true);
    }
}