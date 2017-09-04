using System;
using System.Net.Mime;
using System.Reflection;
using Astral.Settings;

namespace Astral
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Property)]
    public class SerializeToAttribute : Attribute, IAstralAttribute
    {
        public SerializeToAttribute(string contentType)
        {
            ContentType = new ContentType(contentType);
        }

        public ContentType ContentType { get; }

        public (Type, object) GetConfigElement(MemberInfo applyedTo)
            => (typeof(SerailizationContentType), new SerailizationContentType(ContentType));
    }
}