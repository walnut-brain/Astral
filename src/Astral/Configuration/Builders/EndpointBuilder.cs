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
            LawBookBuilder Simple()
                =>  BookBuilder.GetSubBookBuilder((channelKind, isReponse), bld =>
                    {
                        bld.RegisterLaw(Law.Axiom(new SubscribeChannel(channelKind)));
                        bld.RegisterLaw(Law.Axiom(new IsResponseChannel(isReponse)));
                    });
            

            return new ChannelBuilder(channelKind.Match(
                Simple, 
                name =>
                    name == ConfigUtils.DefaultNamedChannel.Name 
                        ? BookBuilder.GetSubBookBuilder((channelKind, isReponse), bld =>
                        {
                            bld.RegisterLaw(Law.Axiom(new IsResponseChannel(isReponse)));
                        })
                        : BookBuilder.GetSubBookBuilder((channelKind, isReponse), bld =>
                        {
                            bld.RegisterLaw(Law.Axiom(new IsResponseChannel(isReponse)));
                        }).GetSubBookBuilder(name, bld => bld.RegisterLaw(Law.Axiom(new SubscribeChannel(channelKind)))),
                Simple,
                Simple,
                Simple,
                (a, b) => Simple(),
                Simple));
        }
        
    }
}