using System.Reflection;
using Lawium;

namespace Astral.Configuration.Settings
{
    public sealed class EndpointProperty : Fact<PropertyInfo>
    {
        public EndpointProperty(PropertyInfo value) : base(value)
        {
        }
    }
}