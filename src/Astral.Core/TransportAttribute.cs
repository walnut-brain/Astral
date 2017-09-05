using System;
using System.Net.Mime;
using System.Reflection;
using Astral.Configuration.Settings;

namespace Astral
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Property)]
    public class TransportAttribute : Attribute, IAstralAttribute
    {
        public TransportAttribute(ContentType contentType) : this("", contentType)
        {
        }

        public TransportAttribute(string tag, ContentType contentType = null)
        {
            Tag = string.IsNullOrWhiteSpace(tag) ? "" : tag;
            ContentType = contentType;
        }

        public string Tag { get; }
        public ContentType ContentType { get; }

        public (Type, object) GetConfigElement(MemberInfo applyedTo)
        {
            return (typeof(TransportSelector), new TransportSelector((Tag, ContentType)));
        }
    }
}