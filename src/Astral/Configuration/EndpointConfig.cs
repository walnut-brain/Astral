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
        internal EndpointConfig(LawBook<Fact> lawBook, IServiceProvider serviceProvider) : base(lawBook, serviceProvider)
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

        


        public Type ServiceType => this.GetRequiredService<ServiceTypeSetting>().Value;
        public string ServiceName => this.GetRequiredService<ServiceName>().Value;
        public PropertyInfo PropertyInfo => this.GetRequiredService<EndpointMemberSetting>().Value;
        public EndpointType EndpointType => this.GetRequiredService<EndpointTypeSetting>().Value;
        public Type MessageType => this.GetRequiredService<MessageTypeSetting>().Value;
        public string EndpointName => this.GetRequiredService<EndpointName>().Value;

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
            LawBook<Fact> Simple() =>
                LawBook.GetOrAddSubBook((channelKind, isResponse), bld =>
                {
                    bld.RegisterLaw(Law<Fact>.Axiom(new SubscribeChannelSetting(channelKind)));
                    bld.RegisterLaw(Law<Fact>.Axiom(new IsResponseChannelSetting(isResponse)));
                }).Result.GetOrAddSubBook(Guid.NewGuid(), b => onCreate(new ChannelBuilder(b))).Result;
            return new ChannelConfig(channelKind.Match(Simple, 
                name => LawBook.GetOrAddSubBook((ConfigUtils.DefaultNamedChannel, isResponse), bld =>
            {
                bld.RegisterLaw(Law<Fact>.Axiom(new IsResponseChannelSetting(isResponse)));
            }).Result.GetOrAddSubBook(name, bld =>
            {
                bld.RegisterLaw(Law<Fact>.Axiom(new SubscribeChannelSetting(channelKind)));
            }).Result.GetOrAddSubBook(Guid.NewGuid(), b => onCreate(new ChannelBuilder(b))).Result, Simple, Simple, Simple, (s, s1) => Simple(), Simple), this);
        }
    }
}