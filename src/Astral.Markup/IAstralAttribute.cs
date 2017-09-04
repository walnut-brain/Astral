using System;
using System.Reflection;

namespace Astral
{
    public interface IAstralAttribute
    {
        (Type, object) GetConfigElement(MemberInfo applyedTo);
    }
}