using System;
using System.Net.Mime;
using System.Reflection;
using Astral.Configuration.Settings;

namespace Astral
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Property)]
    public class RpcTransportAttribute : Attribute, IAstralAttribute
    {
        public RpcTransportAttribute(ContentType contentType) : this("", contentType)
        {
        }

        public RpcTransportAttribute(string tag, ContentType contentType = null)
        {
            Tag = string.IsNullOrWhiteSpace(tag) ? "" : tag;
            ContentType = contentType;
        }

        public string Tag { get; }
        public ContentType ContentType { get; }

        public (Type, object) GetConfigElement(MemberInfo applyedTo)
        {
            return (typeof(RpcTransportSelector), new RpcTransportSelector((Tag, ContentType)));
        }
    }
}