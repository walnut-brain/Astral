using System;
using System.Net.Mime;
using System.Reflection;
using Astral.Configuration;
using Astral.Configuration.Settings;
using Astral.Schema;

namespace Astral
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Property)]
    public class RpcTransportAttribute : Attribute, IConfigAttribute, ISchemaMemberAttribute
    {
        public RpcTransportAttribute(string contentType) : this("", contentType)
        {
            
        }

        public RpcTransportAttribute(string tag, string contentType = null)
        {
            Tag = string.IsNullOrWhiteSpace(tag) ? "" : tag;
            if (Tag == "" && contentType == null) throw new ArgumentNullException(nameof(contentType));
            ContentType = contentType != null ? new ContentType(contentType) : null;
        }

        public string Tag { get; }
        public ContentType ContentType { get; }

        public Fact[] GetConfigElements(MemberInfo applyedTo)
        {
            return new Fact[] {new RpcTransportSelector((Tag, ContentType))};
        }

        public SchemaRecord[] GetSchemaRecords(MemberInfo memberInfo)
        {
            return new[] { new SchemaRecord(null, "rpcTransport", (Tag, ContentType.ToString())) };
        }
    }
}