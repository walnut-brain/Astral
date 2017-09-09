using System;
using System.Reflection;
using Astral.Configuration;
using Astral.Configuration.Settings;
using Lawium;
using Microsoft.Extensions.DependencyInjection;

namespace Astral.Specifications
{
    public class EndpointSpecification : SpecificationBase
    {
        private readonly Lazy<TransportSpecification> _lazyTransport; 
        
        internal EndpointSpecification(LawBook<Fact> lawBook, IServiceProvider serviceProvider) : base(lawBook, serviceProvider)
        {
            _lazyTransport = new Lazy<TransportSpecification>(() => new TransportSpecification(this));
        }

        public Type ServiceType => this.GetRequiredService<ServiceType>().Value;
        public string ServiceName => this.GetRequiredService<ServiceName>().Value;
        public PropertyInfo PropertyInfo => this.GetRequiredService<EndpointMember>().Value;
        public EndpointType EndpointType => this.GetRequiredService<EndpointKind>().Value;
        public Type MessageType => this.GetRequiredService<MessageType>().Value;
        public string EndpointName => this.GetRequiredService<EndpointName>().Value;

        internal TransportSpecification Transport => _lazyTransport.Value;
        
        /*
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
        }*/
    }
}