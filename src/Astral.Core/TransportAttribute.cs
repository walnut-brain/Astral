using System;
using System.Net.Mime;
using System.Reflection;
using Astral.Configuration;
using Astral.Configuration.Settings;
using Astral.Schema;

namespace Astral
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Property)]
    [ConfigRegister]
    public class TransportAttribute : Attribute, ISchemaMemberAttribute
    {
        public TransportAttribute(string contentType) : this("", contentType)
        {
        }

        public TransportAttribute(string tag, string contentType = null)
        {
            Tag = string.IsNullOrWhiteSpace(tag) ? "" : tag;
            if (Tag == "" && contentType == null) throw new ArgumentNullException(nameof(contentType));
            ContentType = contentType != null ? new ContentType(contentType) : null;
        }

        public string Tag { get; }
        public ContentType ContentType { get; }

        public SchemaRecord[] GetSchemaRecords(MemberInfo memberInfo)
            => new[] { new SchemaRecord(null, "transport", (Tag, ContentType.ToString())) };
    }
}