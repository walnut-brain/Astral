using System;
using System.Net.Mime;
using System.Reflection;
using Astral.Configuration.Settings;
using Astral.Exceptions;
using Astral.Payloads.DataContracts;
using Astral.Payloads.Serialization;
using Astral.Transport;
using FunEx;
using Lawium;
using Microsoft.Extensions.DependencyInjection;

namespace Astral.Configuration.Configs
{
    public class EndpointConfig : ConfigBase
    {
        internal EndpointConfig(LawBook lawBook, IServiceProvider serviceProvider) : base(lawBook, serviceProvider.GetService)
        {
        }

        public Type ServiceType => this.Get<ServiceType>().Value;
        public string ServiceName => this.Get<ServiceName>().Value;
        public PropertyInfo PropertyInfo => this.Get<EndpointMember>().Value;
        public EndpointType EndpointType => this.Get<EndpointType>();
        public Type MessageType => this.Get<MessageType>().Value;
        public string EndpointName => this.Get<EndpointName>().Value;

        internal (ITransport, string, ContentType) Transport
        {
            get
            {
                var selector = TryGet<TransportSelector>().Map(p => p.Value);
                var tag = selector.Map(p => ConfigUtils.NormalizeTag(p.Item1)).IfNone(() => ConfigUtils.NormalizeTag(null));
                var contentType = selector.Map(p => p.Item2).OrElse(() => TryGet<SerailizationContentType>().Map(p => p.Value))
                    .Unwrap(new InvalidConfigurationException($"For {ServiceType}  {PropertyInfo.Name} not setted content type of transport"));
                return (this.GetService<TransportProvider>().GetTransport(tag).Unwrap(), tag, contentType);

            }
        }
        internal (IRpcTransport, string, ContentType) RpcTransport 
        {
            get
            {
                var selector = TryGet<RpcTransportSelector>().Map(p => p.Value).OrElse(() => TryGet<TransportSelector>().Map(p => p.Value));
                var tag = selector.Map(p => ConfigUtils.NormalizeTag(p.Item1)).IfNone(() => ConfigUtils.NormalizeTag(null));
                var contentType = selector.Map(p => p.Item2).OrElse(() => TryGet<SerailizationContentType>().Map(p => p.Value))
                    .Unwrap(new InvalidConfigurationException($"For {ServiceType}  {PropertyInfo.Name} not setted content type of transport {tag}"));
                return (this.GetService<TransportProvider>().GetRpcTransport(tag).Unwrap(), tag, contentType);

            }
        }
    }
}