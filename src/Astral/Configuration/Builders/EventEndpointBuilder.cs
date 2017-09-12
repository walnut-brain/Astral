using System;
using Astral.Transport;
using Lawium;
using Microsoft.Extensions.Logging;

namespace Astral.Configuration.Builders
{
    public class EventEndpointBuilder<TEvent> : EndpointBuilder
    {
        internal EventEndpointBuilder(LawBookBuilder<Fact> bookBuilder) : base(bookBuilder)
        {
        }

        public ChannelBuilder SystemChannel => ChannelBuilder(SubscribeChannel.System, false);
        public ChannelBuilder InstanceChannel => ChannelBuilder(SubscribeChannel.Instance, false);
        public ChannelBuilder DedicatedChannel => ChannelBuilder(SubscribeChannel.Dedicated, false);
        public ChannelBuilder NamedBuilder(string name = "<<default>>") => ChannelBuilder(SubscribeChannel.Named(name), false);
    }
}