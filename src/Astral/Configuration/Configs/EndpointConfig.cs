using System;
using System.Net.Mime;
using System.Reflection;
using Astral.Configuration.Settings;
using Astral.Payloads.DataContracts;
using Astral.Payloads.Serialization;
using Astral.Transport;
using Lawium;

namespace Astral.Configuration.Configs
{
    public class EndpointConfig : ConfigBase
    {
        public TypeEncoding TypeEncoding { get; }
        public Serializer<byte[]> Serializer { get; }
        internal TransportProvider Transports { get; }

        internal EndpointConfig(LawBook lawBook, TypeEncoding typeEncoding, Serializer<byte[]> serializer, TransportProvider transports) : base(lawBook)
        {
            TypeEncoding = typeEncoding;
            Serializer = serializer;
            Transports = transports;
        }

        public Type ServiceType => this.Get<ServiceType>().Value;
        public string ServiceName => this.Get<ServiceName>().Value;
        public PropertyInfo PropertyInfo => this.Get<EndpointMember>().Value;
        public EndpointType EndpointType => this.Get<EndpointType>();
        public Type MessageType => this.Get<MessageType>().Value;
        public string EndpointName => this.Get<EndpointName>().Value;
        internal ITransport Transport => throw new NotImplementedException();
        public ContentType ContentType => throw new NotImplementedException();
    }
}