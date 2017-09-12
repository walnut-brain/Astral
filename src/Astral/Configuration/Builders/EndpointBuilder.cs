using System;
using Astral.Configuration.Settings;
using Astral.Transport;
using Lawium;
using Microsoft.Extensions.Logging;

namespace Astral.Configuration.Builders
{
    public abstract class EndpointBuilder : BuilderBase
    {
        protected EndpointBuilder(LawBookBuilder<Fact> bookBuilder) : base(bookBuilder)
        {
        }

        protected ChannelBuilder ChannelBuilder(SubscribeChannel channel, bool isReponse)
        {
            LawBookBuilder<Fact> Simple()
                =>  BookBuilder.GetSubBookBuilder((channel, isReponse), bld =>
                    {
                        bld.RegisterLaw(Law<Fact>.Axiom(new SubscribeChannelSetting(channel)));
                        bld.RegisterLaw(Law<Fact>.Axiom(new IsResponseChannelSetting(isReponse)));
                    });
            

            return new ChannelBuilder(channel.Match(Simple, name =>
                    BookBuilder.GetSubBookBuilder((SubscribeChannel.Named("<<default>>"), isReponse))
                        .GetSubBookBuilder((channel, isReponse)), Simple, Simple, Simple));
        }
        
    }
}