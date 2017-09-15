using System;
using Astral.Configuration.Settings;
using Astral.Transport;
using Lawium;
using Microsoft.Extensions.Logging;

namespace Astral.Configuration.Builders
{
    public abstract class EndpointBuilder : BuilderBase
    {
        protected EndpointBuilder(LawBookBuilder bookBuilder) : base(bookBuilder)
        {
        }

        protected ChannelBuilder ChannelBuilder(ChannelKind channelKind, bool isReponse)
        {
            switch (channelKind)
            {
                case ChannelKind.NamedChannelKind nck:
                    if(nck.Name == ConfigUtils.DefaultNamedChannel.Name)
                        return new ChannelBuilder(BookBuilder.GetSubBookBuilder((channelKind, isReponse), bld =>
                        {
                            bld.RegisterLaw(Law.Axiom(new IsResponseChannel(isReponse)));
                        }));
                    return new ChannelBuilder(BookBuilder.GetSubBookBuilder((channelKind, isReponse), bld =>
                    {
                        bld.RegisterLaw(Law.Axiom(new IsResponseChannel(isReponse)));
                    }).GetSubBookBuilder(nck.Name, bld => bld.RegisterLaw(Law.Axiom(new SubscribeChannel(channelKind)))));
                default:
                    return new ChannelBuilder(BookBuilder.GetSubBookBuilder((channelKind, isReponse), bld =>
                    {
                        bld.RegisterLaw(Law.Axiom(new SubscribeChannel(channelKind)));
                        bld.RegisterLaw(Law.Axiom(new IsResponseChannel(isReponse)));
                    }));
            }
        }
        
    }
}