using System;
using System.Reflection;

namespace Astral.Configuration
{
    public interface IConfigAttribute
    {
        Fact[] GetConfigElements(MemberInfo applyedTo);
    }
}