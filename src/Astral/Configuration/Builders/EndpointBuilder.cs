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
                        bld.RegisterLaw(Law.Axiom(new SubscribeChannelSetting(channelKind)));
                        bld.RegisterLaw(Law.Axiom(new IsResponseChannelSetting(isReponse)));
                    });
            

            return new ChannelBuilder(channelKind.Match(
                Simple, 
                name =>
                    name == ConfigUtils.DefaultNamedChannel.Name 
                        ? BookBuilder.GetSubBookBuilder((channelKind, isReponse), bld =>
                        {
                            bld.RegisterLaw(Law.Axiom(new IsResponseChannelSetting(isReponse)));
                        })
                        : BookBuilder.GetSubBookBuilder((channelKind, isReponse), bld =>
                        {
                            bld.RegisterLaw(Law.Axiom(new IsResponseChannelSetting(isReponse)));
                        }).GetSubBookBuilder(name, bld => bld.RegisterLaw(Law.Axiom(new SubscribeChannelSetting(channelKind)))),
                Simple,
                Simple,
                Simple,
                (a, b) => Simple(),
                Simple));
        }
        
    }
}