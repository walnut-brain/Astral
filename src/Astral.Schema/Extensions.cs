using System.Net.Mime;

namespace Astral.Schema
{
    public static class Extensions
    {
       
        
        public static ContentType ContentType(this RootSchema schema)
            => schema.TryGetProperty<ContentType>(nameof(ContentType)).IfNone(() => new ContentType("text/json;charset=utf-8"));

        public static RootSchema ContentType(this RootSchema schema, ContentType value)
        {
            schema.SetProperty(nameof(ContentType), value);
            return schema;
        }
        
        public static ContentType ContentType<T>(this EndpointSchema<T> schema) 
            where T : EndpointSchema<T> 
            => schema.TryGetProperty<ContentType>(nameof(ContentType))
                .IfNone(schema.Service.ContentType);

        public static T ContentType<T>(this T schema, ContentType value)
            where T : EndpointSchema<T> => schema.SetProperty(nameof(ContentType), value);
    }
}