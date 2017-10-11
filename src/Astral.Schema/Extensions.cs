using System.Linq;
using System.Net.Mime;
using Astral.Markup.RabbitMq;
using Astral.Schema.Data;
using Astral.Schema.RabbitMq;

namespace Astral.Schema
{
    public static class Extensions
    {
        public static bool HasContentType(this ISchema schema)
            => schema.TryGetProperty<ContentType>(nameof(ContentType)).Map(p => true).IfNone(false);
        
        public static ContentType ContentType(this IComplexServiceSchema schema)
            => schema.TryGetProperty<ContentType>(nameof(ContentType)).IfNone(() => new ContentType("text/json;charset=utf-8"));
        
        public static ContentType ContentType(this IServiceSchema schema)
            => schema.TryGetProperty<ContentType>(nameof(ContentType)).IfNone(() => new ContentType("text/json;charset=utf-8"));

        public static RootSchema ContentType(this RootSchema schema, ContentType value)
        {
            schema.SetProperty(nameof(ContentType), value);
            return schema;
        }
        
        public static ContentType ContentType(this IEndpointSchema schema) 
            => schema.TryGetProperty<ContentType>(nameof(ContentType))
                .IfNone(schema.Service.ContentType);

        public static T ContentType<T>(this T schema, ContentType value)
            where T : EndpointSchema<T> => schema.SetProperty(nameof(ContentType), value);

        
    }
}