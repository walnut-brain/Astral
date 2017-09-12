using System.Reflection;
using Lawium;

namespace Astral.Configuration.Settings
{
    public sealed class EndpointMemberSetting : Fact<PropertyInfo>
    {
        public EndpointMemberSetting(PropertyInfo value) : base(value)
        {
        }
    }
}