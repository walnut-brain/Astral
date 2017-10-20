using System.Collections.Generic;
using System.Linq;
using Astral.Markup.RabbitMq;
using Astral.Schema.Data;
using Astral.Schema.RabbitMq;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace Astral.Schema.Json
{
    public static class Extensions
    {
        public static string ToYaml(this ServiceSchema schema)
        {
            Option<YamlNode> ExchangeToYaml(ExchangeSchema exchange)
            {
                if (!exchange.AutoDelete && !exchange.Delayed && exchange.Durable && exchange.Alternate == null)
                {
                    if (exchange.Name == null)
                    {
                        if (exchange.Type == ExchangeKind.Direct)
                            return Option.None;
                        return new YamlScalarNode(exchange.Type.ToString().ToLowerInvariant());
                    }
                    if (exchange.Type == ExchangeKind.Direct)
                        return  new YamlScalarNode(exchange.Name);
                }
                var obj = new YamlMappingNode();
                if(exchange.Name != null)
                    obj.Add("name", exchange.Name);
                if (exchange.Type != ExchangeKind.Direct)
                    obj.Add("type", exchange.Type.ToString().ToLowerInvariant());
                if (exchange.AutoDelete)
                    obj.Add("autoDelete", new YamlScalarNode("true"));
                if (exchange.Delayed)
                    obj.Add("delayed", new YamlScalarNode("false"));
                if (!exchange.Durable)
                    obj.Add("durable", new YamlScalarNode("false"));
                if (exchange.Alternate != null)
                    obj.Add("alternate", exchange.Alternate);
                return obj;
            }

            Option<YamlNode> RequestQueueToYaml(RequestQueueSchema queue)
            {
                if (queue.AutoDelete && !queue.Durable)
                {
                    if (queue.Name != null)
                    {

                        return new YamlScalarNode(queue.Name);
                    }
                    return Option.None;
                }
                var obj = new YamlMappingNode();
                if (queue.Name != null)
                    obj.Add("name", queue.Name);
                if (!queue.AutoDelete)
                    obj.Add("autoDelete", "false");
                if (queue.Durable)
                    obj.Add("durable", "true");
                return obj;
            }

            YamlMappingNode EventToYaml(EventSchema eventSchema)
            {
                var obj = new YamlMappingNode
                {
                    {"contract", eventSchema.EventType.SchemaName}, 
                    {"title", eventSchema.CodeName}
                };
                if(eventSchema.HasExchange)
                    ExchangeToYaml(eventSchema.Exchange).IfSome(p => obj.Add("exchange", p));
                if(eventSchema.HasRoutingKey)
                    obj.Add("routingKey", eventSchema.RoutingKey);
                if(eventSchema.HasContentType)
                    obj.Add("contentType", eventSchema.ContentType.ToString());
                return obj;
            }
            
            YamlMappingNode CallToYaml(CallSchema callSchema)
            {
                var obj = new YamlMappingNode
                {
                    {"request", callSchema.RequestType.SchemaName},
                    {"title", callSchema.CodeName}
                };
                if (callSchema.ResponseType != null)
                    obj.Add("response", callSchema.ResponseType.SchemaName);
                
                if(callSchema.HasExchange)
                    ExchangeToYaml(callSchema.Exchange).IfSome(p => obj.Add("exchange", p));
                if(callSchema.HasResponseExchange)
                    ExchangeToYaml(callSchema.ResponseExchange).IfSome(p => obj.Add("responseExchange", p));
                if(callSchema.HasRoutingKey)
                    obj.Add("routingKey", callSchema.RoutingKey);
                if(callSchema.HasContentType)
                    obj.Add("contentType", callSchema.ContentType.ToString());
                if(callSchema.HasRequestQueue)
                    RequestQueueToYaml(callSchema.RequestQueue).IfSome(p => obj.Add("queue", p));
                return obj;
            }

            YamlMappingNode TypeToYaml(ITypeSchema desc)
            {
                var obj = new YamlMappingNode();
                if(desc.ContractName != null)
                    obj.Add("contract", desc.ContractName);
                if(desc.CodeName != null && desc.CodeName != desc.SchemaName)
                    obj.Add("title", desc.CodeName);
                switch (desc)
                {
                    case IComplexTypeSchema complexTypeDesc:
                        
                        if (complexTypeDesc.BaseOn != null)
                            obj.Add("base", complexTypeDesc.BaseOn.SchemaName);
                        if (complexTypeDesc.IsStruct)
                            obj.Add("struct", "true");
                        obj.Add("fields", new YamlMappingNode(complexTypeDesc.Fields.Select(p => new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(p.Key),new YamlScalarNode(p.Value.SchemaName) ))));
                        return obj;
                    case IEnumTypeSchema enumTypeDesc:
                        obj.Add("base", enumTypeDesc.BaseOn.SchemaName);
                        if(enumTypeDesc.IsFlags)
                            obj.Add("falgs", "true");
                        obj.Add("values", new YamlMappingNode(enumTypeDesc.Values.Select(p => new KeyValuePair<YamlNode, YamlNode>(new YamlScalarNode(p.Key), new YamlScalarNode(p.Value.ToString())))));
                        return obj;
                    default:
                        throw new SchemaException("Unknown type description");
                }
            }

            var yamlRoot = new YamlMappingNode
            {
                {"name", schema.Name}, 
                {"owner", schema.Owner}, 
                {"title", schema.CodeName}
            };
            if(schema.HasContentType)
                yamlRoot.Add("content", schema.ContentType.ToString());
            if (schema.HasExchange)
                ExchangeToYaml(schema.Exchange).IfSome(p => yamlRoot.Add("exchange", p));
            if (schema.HasResponseExchange)
                ExchangeToYaml(schema.ResponseExchange).IfSome(p => yamlRoot.Add("responseExchange", p));
            yamlRoot.Add("events", new YamlMappingNode(schema.Events.Select(p => new KeyValuePair<YamlNode, YamlNode>(p.Name, EventToYaml(p)))));
            yamlRoot.Add("calls", new YamlMappingNode(schema.Calls.Select(p => new KeyValuePair<YamlNode, YamlNode>(p.Name, CallToYaml(p)))));
            yamlRoot.Add("enums", new YamlMappingNode(schema.Types.OfType<IEnumTypeSchema>().Select(p => new KeyValuePair<YamlNode, YamlNode>(p.SchemaName, TypeToYaml(p)))));
            yamlRoot.Add("types", new YamlMappingNode(schema.Types.OfType<IComplexTypeSchema>().Select(p => new KeyValuePair<YamlNode, YamlNode>(p.SchemaName, TypeToYaml(p)))));
            var serializer = new SerializerBuilder().Build();
            
            return serializer.Serialize(yamlRoot);

        }
    }
}