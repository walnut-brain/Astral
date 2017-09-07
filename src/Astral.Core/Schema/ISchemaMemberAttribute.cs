using System.Reflection;

namespace Astral.Schema
{
    public interface ISchemaMemberAttribute
    {
        SchemaRecord[] GetSchemaRecords(MemberInfo memberInfo);
    }
}