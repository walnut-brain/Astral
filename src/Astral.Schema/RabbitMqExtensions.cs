using System;
using Astral.Markup.RabbitMq;
using Astral.Schema.RabbitMq;

namespace Astral.Schema
{
    public static class RabbitMqExtensions
    {
        public static bool HasExchange(this ISchema schema)
            => schema.TryGetProperty<ExchangeSchema>(nameof(Exchange)).IsSome;
        
        public static ExchangeSchema Exchange(this IComplexServiceSchema schema) =>
            schema.TryGetProperty<ExchangeSchema>(nameof(Exchange))
                .Map(p => string.IsNullOrWhiteSpace(p.Name) ? new ExchangeSchema($"{schema.Owner}.{schema.Name}", p.Type,
                    p.Durable, p.AutoDelete, p.Delayed, p.Alternate) : p)
                .IfNone(() => new ExchangeSchema($"{schema.Owner}.{schema.Name}"));
        
        public static ExchangeSchema Exchange(this IServiceSchema schema) =>
            schema.TryGetProperty<ExchangeSchema>(nameof(Exchange))
                .Map(p => string.IsNullOrWhiteSpace(p.Name) ? new ExchangeSchema($"{schema.Owner}.{schema.Name}", p.Type,
                        p.Durable, p.AutoDelete, p.Delayed, p.Alternate) : p)
                .IfNone(() => new ExchangeSchema($"{schema.Owner}.{schema.Name}"));

        public static RootSchema Exchange(this RootSchema schema, ExchangeSchema exchange)
        {
            if(exchange != null && exchange.Type == ExchangeKind.Fanout)
                throw new SchemaException($"Exchange specified for {(schema.ServiceType() == null ? schema.Name : schema.ServiceType().Name)} cannot be Fanout");
            return schema.SetProperty(nameof(Exchange), exchange);
            
        }

        public static ExchangeSchema Exchange<T>(this T schema) 
            where T : IEndpointSchema 
            =>
                schema.TryGetProperty<ExchangeSchema>(nameof(Exchange))
                    .Map(p => string.IsNullOrWhiteSpace(p.Name)
                        ? new ExchangeSchema($"{schema.Service.Owner}.{schema.Service.Name}.{schema.Name}", p.Type,
                            p.Durable, p.AutoDelete, p.Delayed, p.Alternate)
                        : p)
                    .IfNone(schema.Service.Exchange);
        
        public static T Exchange<T>(this T schema, ExchangeSchema exchange)
            where T : EndpointSchema<T> => schema.SetProperty(nameof(Exchange), exchange);


        public static bool HasResponseExchange(this ISchema schema) =>
            schema.TryGetProperty<ExchangeSchema>(nameof(ResponseExchange)).IsSome;
        
        public static ExchangeSchema ResponseExchange(this IServiceSchema schema) =>
            schema.TryGetProperty<ExchangeSchema>(nameof(ResponseExchange))
                .Map(p => string.IsNullOrWhiteSpace(p.Name) 
                    ? p.Name == null 
                           ? new ExchangeSchema($"{schema.Owner}.{schema.Name}.responses", p.Type, p.Durable, p.AutoDelete, p.Delayed, p.Alternate)
                           : p
                    : p)
                .IfNone(schema.Exchange);

        public static RootSchema ResponseExchange(this RootSchema schema, ExchangeSchema exchange)
        {
            if(exchange != null && exchange.Type == ExchangeKind.Fanout)
                throw new SchemaException($"Response exchange specified for {(schema.ServiceType() == null ? schema.Name : schema.ServiceType().Name)} cannot be Fanout");
            return schema.SetProperty(nameof(ResponseExchange), exchange);
        }

        public static ExchangeSchema ResponseExchange(this ICallSchema schema) =>
            schema.TryGetProperty<ExchangeSchema>(nameof(ResponseExchange))
                .Map(p => string.IsNullOrWhiteSpace(p.Name)
                    ? p.Name == null 
                        ? new ExchangeSchema($"{schema.Service.Owner}.{schema.Service.Name}.{schema.Name}.responses", p.Type, p.Durable, p.AutoDelete, p.Delayed, p.Alternate)
                        : p 
                    : p)
                .IfNone(schema.Service.ResponseExchange);
        
        public static CallSchema ResponseExchange(this CallSchema schema, ExchangeSchema exchange)
        {
            if(exchange != null && exchange.Type == ExchangeKind.Fanout)
                throw new SchemaException(
                    $"Response exchange specified for call {schema.CodeName() ?? schema.Name} on service {(schema.Service.ServiceType() == null ? schema.Service.Name : schema.Service.ServiceType().Name)} cannot be Fanout");
            schema.SetProperty(nameof(ResponseExchange), exchange);
            return schema;
        }

        public static bool HasRoutingKey<T>(this T schema)
            where T : IEndpointSchema
            => schema.TryGetProperty<string>(nameof(RoutingKey)).IsSome;
        
        public static string RoutingKey<T>(this T schema)
            where T : IEndpointSchema
            => schema.TryGetProperty<string>(nameof(RoutingKey)).IfNone(schema.Name);

        public static T RoutingKey<T>(this T schema, string value)
            where T : EndpointSchema<T> => schema.SetProperty(nameof(RoutingKey), value);


        public static bool HasRequestQueue(this ICallSchema schema)
            => schema
                .TryGetProperty<RequestQueueSchema>(nameof(RequestQueue)).IsSome;
        
        public static RequestQueueSchema RequestQueue(this ICallSchema schema)
            => schema
                .TryGetProperty<RequestQueueSchema>(nameof(RequestQueue))
                .Map(p => p.Name == null
                    ? new RequestQueueSchema($"{schema.Service.Owner}.{schema.Service.Name}.{schema.Name}",
                        p.Durable, p.AutoDelete)
                    : p)
                .IfNone(() =>
                    new RequestQueueSchema($"{schema.Service.Owner}.{schema.Service.Name}.{schema.Name}"));
        
        
        public static CallSchema RequestQueue(this CallSchema schema, RequestQueueSchema parameters) 
            => schema.SetProperty(nameof(RequestQueue), parameters);
    }
}