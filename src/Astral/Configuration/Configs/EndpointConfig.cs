using System;
using System.Reflection;
using Astral.Configuration.Settings;
using Astral.Lawium;

namespace Astral.Configuration.Configs
{
    public class EndpointConfig : ConfigBase
    {
        internal EndpointConfig(LawBook lawBook) : base(lawBook)
        {
        }

        public Type ServiceType => this.Get<ServiceType>().Value;
        public string ServiceName => this.Get<ServiceName>().Value;
        public PropertyInfo PropertyInfo => this.Get<EndpointMember>().Value;
        public EndpointType EndpointType => this.Get<EndpointType>();
        public Type MessageType => this.Get<MessageType>().Value;
        public string EndpointName => this.Get<EndpointName>().Value;
    }
}