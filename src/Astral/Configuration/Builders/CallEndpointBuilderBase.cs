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
        
        public ChannelBuilder SystemChannel => ChannelBuilder(ChannelKind.System, false);
        public ChannelBuilder InstanceChannel => ChannelBuilder(ChannelKind.Instance, false);
        public ChannelBuilder DedicatedChannel => ChannelBuilder(ChannelKind.Dedicated, false);
        public ChannelBuilder NamedBuilder(string name = null) => 
            string.IsNullOrEmpty(name) ? ChannelBuilder(ConfigUtils.DefaultNamedChannel, false) : ChannelBuilder(ChannelKind.Named(name), false);
        
        
        public ChannelBuilder ResponseSystemChannel => ChannelBuilder(ChannelKind.System, true);
        public ChannelBuilder ResponseInstanceChannel => ChannelBuilder(ChannelKind.Instance, true);
        public ChannelBuilder ResponseDedicatedChannel => ChannelBuilder(ChannelKind.Dedicated, true);
        public ChannelBuilder ResponseNamedBuilder(string name = null) => 
            string.IsNullOrEmpty(name) ? ChannelBuilder(ConfigUtils.DefaultNamedChannel, true) : ChannelBuilder(ChannelKind.Named(name), true);
        public ChannelBuilder ResponseRpcChannel => ChannelBuilder(ChannelKind.Rpc, true);
    }
}