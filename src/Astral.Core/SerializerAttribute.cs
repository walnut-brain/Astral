using System;
using System.Net.Mime;
using System.Reflection;
using Astral.Configuration;
using Astral.Configuration.Settings;
using Astral.Schema;

namespace Astral
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Property)]
    public class SerializeToAttribute : Attribute, IConfigAttribute, ISchemaMemberAttribute
    {
        public SerializeToAttribute(string contentType)
        {
            ContentType = new ContentType(contentType);
        }

        public ContentType ContentType { get; }

        public Fact[] GetConfigElements(MemberInfo applyedTo)
            => new Fact[] {new SerailizationContentType(ContentType)};

        public SchemaRecord[] GetSchemaRecords(MemberInfo memberInfo)
            => new[] {new SchemaRecord(null, "contentType", ContentType.ToString())};
    }
}