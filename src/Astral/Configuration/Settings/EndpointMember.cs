using System.Reflection;
using LanguageExt;

namespace Astral.Configuration.Settings
{
    public class EndpointMember : NewType<EndpointMember, PropertyInfo>
    {
        public EndpointMember(PropertyInfo value) : base(value)
        {
        }
    }
}