using System;
using System.Reflection;

namespace Astral
{
    public interface IAstralAttribute
    {
        object GetConfigElement(MemberInfo applyedTo);
    }
}