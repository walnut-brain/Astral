using System;
using Astral.Transport;
using Lawium;
using Microsoft.Extensions.Logging;

namespace Astral.Configuration.Builders
{
    public class EventEndpointBuilder<TEvent> : EndpointBuilder
    {
        internal EventEndpointBuilder(LawBookBuilder bookBuilder) : base(bookBuilder)
        {
        }

        public ChannelBuilder SystemChannel => ChannelBuilder(ChannelKind.System, false);
        public ChannelBuilder InstanceChannel => ChannelBuilder(ChannelKind.Instance, false);
        public ChannelBuilder DedicatedChannel => ChannelBuilder(ChannelKind.Dedicated, false);
        public ChannelBuilder NamedBuilder(string name = null) => 
            string.IsNullOrEmpty(name) ? ChannelBuilder(ConfigUtils.DefaultNamedChannel, false) : ChannelBuilder(ChannelKind.Named(name), false);
    }
}