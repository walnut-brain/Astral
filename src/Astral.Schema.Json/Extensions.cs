﻿using System.Linq;
using Astral.Markup.RabbitMq;
using Astral.Schema.Data;
using Astral.Schema.RabbitMq;
using Newtonsoft.Json.Linq;

namespace Astral.Schema.Json
{
    public static class Extensions
    {
        public static JObject ToJson(this IComplexServiceSchema schema)
        {
            Option<JToken> ExchangeToJson(ExchangeSchema exchange)
            {
                if (!exchange.AutoDelete && !exchange.Delayed && exchange.Durable && exchange.Alternate == null)
                {
                    if (exchange.Name == null)
                    {
                        if (exchange.Type == ExchangeKind.Direct)
                            return Option.None;
                        return JValue.CreateString(exchange.Type.ToString().ToLowerInvariant());
                    }
                    if (exchange.Type == ExchangeKind.Direct)
                        return JValue.CreateString(exchange.Name);
                }
                var obj = new JObject();
                if(exchange.Name != null)
                    obj.Add("name", exchange.Name);
                if (exchange.Type != ExchangeKind.Direct)
                    obj.Add("type", exchange.Type.ToString().ToLowerInvariant());
                if (exchange.AutoDelete)
                    obj.Add("autDelete", true);
                if (exchange.Delayed)
                    obj.Add("delayed", true);
                if (!exchange.Durable)
                    obj.Add("durable", false);
                if (exchange.Alternate != null)
                    obj.Add("alternate", exchange.Alternate);
                return obj;
            }

            Option<JToken> RequestQueueToJson(RequestQueueSchema queue)
            {
                if (queue.AutoDelete && !queue.Durable)
                {
                    if (queue.Name != null)
                    {

                        return JValue.CreateString(queue.Name);
                    }
                    return Option.None;
                }
                var obj = new JObject();
                if (queue.Name != null)
                    obj.Add("name", queue.Name);
                if (!queue.AutoDelete)
                    obj.Add("autoDelete", queue.AutoDelete);
                if (queue.Durable)
                    obj.Add("durable", queue.Durable);
                return obj;
            }

            JObject EventToJson(IEventSchema eventSchema)
            {
                var obj = new JObject
                {
                    {"contract", eventSchema.ContractName()}, 
                    {"title", eventSchema.CodeName()}
                };
                if(eventSchema.HasExchange())
                    ExchangeToJson(eventSchema.Exchange()).IfSome(p => obj.Add("exchange", p));
                if(eventSchema.HasRoutingKey())
                    obj.Add("routingKey", eventSchema.RoutingKey());
                if(eventSchema.HasContentType())
                    obj.Add("contentType", eventSchema.ContentType().ToString());
                return obj;
            }
            
            JObject CallToJson(ICallSchema callSchema)
            {
                var obj = new JObject
                {
                    {"request", callSchema.RequestContract()},
                    {"title", callSchema.CodeName()}
                };
                if (callSchema.ResponseContract() != null)
                    obj.Add("response", callSchema.ResponseContract());
                
                if(callSchema.HasExchange())
                    ExchangeToJson(callSchema.Exchange()).IfSome(p => obj.Add("exchange", p));
                if(callSchema.HasResponseExchange())
                    ExchangeToJson(callSchema.ResponseExchange()).IfSome(p => obj.Add("responseExchange", p));
                if(callSchema.HasRoutingKey())
                    obj.Add("routingKey", callSchema.RoutingKey());
                if(callSchema.HasContentType())
                    obj.Add("contentType", callSchema.ContentType().ToString());
                if(callSchema.HasRequestQueue())
                    RequestQueueToJson(callSchema.RequestQueue()).IfSome(p => obj.Add("queue", p));
                return obj;
            }

            JObject TypeToJson(TypeDesc desc)
            {
                var obj = new JObject();
                switch (desc)
                {
                    case ComplexTypeDesc complexTypeDesc:
                        if(complexTypeDesc.HasContract)
                            obj.Add("contract", complexTypeDesc.Contract);
                        if (complexTypeDesc.Parent != null)
                            obj.Add("base", complexTypeDesc.Parent.Contract);
                        obj.Add("fields", new JObject(complexTypeDesc.Fields.Select(p => new JProperty(p.Key, p.Value.Contract))));
                        return obj;
                    case EnumTypeDesc enumTypeDesc:
                        if(enumTypeDesc.HasContract)
                            obj.Add("contract", enumTypeDesc.Contract);
                        obj.Add("base", enumTypeDesc.BaseType.Contract);
                        obj.Add("values", new JObject(enumTypeDesc.Values.Select(p => new JProperty(p.Key, p.Value))));
                        return obj;
                    default:
                        throw new SchemaException("Unknown type description");
                }
            }

            var jsonSchema = new JObject
            {
                {"name", schema.Name}, 
                {"owner", schema.Owner}, 
                {"title", schema.CodeName()}
            };
            if(schema.HasContentType())
                jsonSchema.Add("content", schema.ContentType().ToString());
            if (schema.HasExchange())
                ExchangeToJson(schema.Exchange()).IfSome(p => jsonSchema.Add("exchange", p));
            if (schema.HasResponseExchange())
                ExchangeToJson(schema.ResponseExchange()).IfSome(p => jsonSchema.Add("responseExchange", p));
            jsonSchema.Add("events", new JObject(schema.Events.Select(p => new JProperty(p.Name, EventToJson(p))).Cast<object>().ToArray()));
            jsonSchema.Add("calls", new JObject(schema.Calls.Select(p => new JProperty(p.Name, CallToJson(p))).Cast<object>().ToArray()));
            jsonSchema.Add("enums", new JObject(schema.Types.OfType<EnumTypeDesc>().Select(p => new JProperty(p.Name, TypeToJson(p))).Cast<object>().ToArray()));
            jsonSchema.Add("types", new JObject(schema.Types.OfType<ComplexTypeDesc>().Select(p => new JProperty(p.Name, TypeToJson(p))).Cast<object>().ToArray()));
            return jsonSchema;

        }
    }
}