using System;
using System.Net.Mime;
using System.Reflection;
using Astral.Configuration;
using Astral.Configuration.Builders;
using Astral.Configuration.Settings;
using Astral.Exceptions;
using Astral.Payloads;
using Astral.Payloads.DataContracts;
using Astral.Payloads.Serialization;
using Astral.Transport;
using FunEx;
using FunEx.Monads;
using Lawium;
using Microsoft.Extensions.DependencyInjection;

namespace Astral.Specifications
{
    public class EndpointConfig : ConfigBase
    {
        internal EndpointConfig(LawBook lawBook, IServiceProvider serviceProvider) : base(lawBook, serviceProvider)
        {
            var selector = this.TryGetService<TransportSelector>().Map(p => p.Value);
            Transport = this
                .GetService<TransportProvider>()
                .GetTransport(selector.Map(p => ConfigUtils.NormalizeTag(p.Item1)).IfNone(() => ConfigUtils.NormalizeTag(null)))
                .Unwrap();
            TransportTag = selector.Map(p => ConfigUtils.NormalizeTag(p.Item1)).IfNone(() => ConfigUtils.NormalizeTag(null));
            ContentType = 
                selector.Map(p => p.Item2).OrElse(() => this.TryGetService<SerailizationContentType>().Map(p => p.Value))
                .Unwrap(new InvalidConfigurationException($"For {ServiceType}  {PropertyInfo.Name} not setted content type of transport"));
        }

        


        public Type ServiceType => this.GetRequiredService<ServiceType>();
        public string ServiceName => this.GetRequiredService<ServiceName>();
        public PropertyInfo PropertyInfo => this.GetRequiredService<EndpointProperty>();
        public EndpointType EndpointType => this.GetRequiredService<EndpointType>();
        public Type MessageType => this.GetRequiredService<MessageType>();
        public string EndpointName => this.GetRequiredService<EndpointName>();

        internal ITransport Transport { get; }
        public string TransportTag { get;  }
        public ContentType ContentType { get; }

        internal PayloadEncode<byte[]> PayloadEncode => new PayloadEncode<byte[]>(ContentType, 
            this.GetService<TypeEncoding>().Encode, this.GetService<Serialization<byte[]>>().Serialize);

        internal Result<Payload<byte[]>> ToPayload<T>(T value) => Payload.ToPayload(Logger, value, PayloadEncode);

        internal Payload.IFromPayload FromPayload(Payload<byte[]> payload) => Payload.FromPayload(Logger, payload,
            new PayloadDecode<byte[]>(
                this.GetService<TypeEncoding>().Decode, this.GetService<Serialization<byte[]>>().Deserialize));
        
        internal ChannelConfig Channel(ChannelKind channelKind, bool isResponse, Action<ChannelBuilder> onCreate)
        {
            LawBook Simple() =>
                LawBook.GetOrAddSubBook((channelKind, isResponse), bld =>
                {
                    bld.RegisterLaw(Law.Axiom(new SubscribeChannel(channelKind)));
                    bld.RegisterLaw(Law.Axiom(new IsResponseChannel(isResponse)));
                }).AddSubBook(b => onCreate(new ChannelBuilder(b)));
            return new ChannelConfig(channelKind.Match(Simple, 
                name => LawBook.GetOrAddSubBook((ConfigUtils.DefaultNamedChannel, isResponse), bld =>
            {
                bld.RegisterLaw(Law.Axiom(new IsResponseChannel(isResponse)));
            }).GetOrAddSubBook(name, bld =>
            {
                bld.RegisterLaw(Law.Axiom(new SubscribeChannel(channelKind)));
            }).AddSubBook(b => onCreate(new ChannelBuilder(b))), Simple, Simple, Simple, (s, s1) => Simple(), Simple), this);
        }
    }
}