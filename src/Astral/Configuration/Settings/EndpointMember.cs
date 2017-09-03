using System.Reflection;
using Lawium;

namespace Astral.Configuration.Settings
{
    public sealed class EndpointMember : Fact<PropertyInfo>
    {
        public EndpointMember(PropertyInfo value) : base(value)
        {
        }
    }
}