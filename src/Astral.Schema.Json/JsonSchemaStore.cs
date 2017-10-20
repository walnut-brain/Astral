using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Mime;
using Astral.Markup.RabbitMq;
using Astral.Schema.Green;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Astral.Schema.Json
{
    public class JsonSchemaStore
    {
        private readonly ServiceSchema _schema;

        internal JsonSchemaStore(ServiceSchema schema)
        {
            _schema = schema;
        }

        public string ToJson()
            => ToJObject().ToString(Formatting.Indented);


        public static ServiceSchema FromJson(string json)
            => FromJObject(JToken.Parse(json));

        private static ExchangeSchema JsonToExchange(JToken token)
        {
            if (token is JValue jvalue)
            {
                try
                {
                    var str = jvalue.ToObject<string>();
                    if (StringComparer.InvariantCultureIgnoreCase.Equals(str, nameof(ExchangeKind.Fanout)))
                        return new ExchangeSchema(null, ExchangeKind.Fanout);
                    if (StringComparer.InvariantCultureIgnoreCase.Equals(str, nameof(ExchangeKind.Topic)))
                        return new ExchangeSchema(null, ExchangeKind.Topic);
                    if (StringComparer.InvariantCultureIgnoreCase.Equals(str, nameof(ExchangeKind.Direct)))
                        return new ExchangeSchema(null);
                    return new ExchangeSchema(str);
                }
                catch (Exception ex)
                {
                    throw new SchemaFormatException($"Invalid token in exchange description at path {jvalue.Path}", ex);
                }
            }

            if (token is JObject job)
            {

                string name = null;
                if (job.TryGetValue("name", StringComparison.InvariantCultureIgnoreCase, out var tkn))
                {
                    try
                    {
                        name = tkn.ToObject<string>();
                    }
                    catch (Exception ex)
                    {
                        throw new SchemaFormatException(
                            $"Invalid token in exchange name description at path {tkn.Path}", ex);
                    }
                }

                var kind = ExchangeKind.Direct;
                if (job.TryGetValue("type", StringComparison.InvariantCultureIgnoreCase, out tkn))
                {
                    try
                    {
                        var str = tkn.ToObject<string>();
                        if (StringComparer.InvariantCultureIgnoreCase.Equals(str, nameof(ExchangeKind.Fanout)))
                            kind = ExchangeKind.Fanout;
                        else if (StringComparer.InvariantCultureIgnoreCase.Equals(str, nameof(ExchangeKind.Direct)))
                            kind = ExchangeKind.Direct;
                        else if (StringComparer.InvariantCultureIgnoreCase.Equals(str, nameof(ExchangeKind.Topic)))
                            kind = ExchangeKind.Topic;
                        else
                            throw new SchemaFormatException($"Unknown exchange type {kind}");
                    }
                    catch (Exception ex)
                    {
                        throw new SchemaFormatException(
                            $"Invalid token in exchange name description at path {tkn.Path}", ex);
                    }
                }

                var autoDelete = false;
                if (job.TryGetValue("autoDelete", StringComparison.InvariantCultureIgnoreCase, out tkn))
                {
                    try
                    {
                        autoDelete = tkn.ToObject<bool>();
                    }
                    catch (Exception ex)
                    {
                        throw new SchemaFormatException(
                            $"Invalid token in exchange auto delete description at path {tkn.Path}", ex);
                    }
                }

                var delayed = false;
                if (job.TryGetValue("delayed", StringComparison.InvariantCultureIgnoreCase, out tkn))
                {
                    try
                    {
                        delayed = tkn.ToObject<bool>();
                    }
                    catch (Exception ex)
                    {
                        throw new SchemaFormatException(
                            $"Invalid token in exchange delayed description at path {tkn.Path}", ex);
                    }
                }

                var durable = true;
                if (job.TryGetValue("durable", StringComparison.InvariantCultureIgnoreCase, out tkn))
                {
                    try
                    {
                        durable = tkn.ToObject<bool>();
                    }
                    catch (Exception ex)
                    {
                        throw new SchemaFormatException(
                            $"Invalid token in exchange durable description at path {tkn.Path}", ex);
                    }
                }

                string alternate = null;
                if (job.TryGetValue("alternate", StringComparison.InvariantCultureIgnoreCase, out tkn))
                {
                    try
                    {
                        alternate = tkn.ToObject<string>();
                    }
                    catch (Exception ex)
                    {
                        throw new SchemaFormatException(
                            $"Invalid token in exchange alternate description at path {tkn.Path}", ex);
                    }
                }

                return new ExchangeSchema(name, kind, durable, autoDelete, delayed, alternate);

            }
            throw new SchemaFormatException($"Invalid token in exchange description at path {token.Path}");
        }

        private static RequestQueueSchema JsonToRequestQueue(JToken token)
        {
            
            if (token is JValue jvalue)
            {
                try
                {
                    var str = jvalue.ToObject<string>();
                    return new RequestQueueSchema(str);
                }
                catch (Exception ex)
                {
                    throw new SchemaFormatException($"Invalid token in request queue description at path {jvalue.Path}", ex);
                }
            }

            if (token is JObject job)
            {

                string name = null;
                if (job.TryGetValue("name", StringComparison.InvariantCultureIgnoreCase, out var tkn))
                {
                    try
                    {
                        name = tkn.ToObject<string>();
                    }
                    catch (Exception ex)
                    {
                        throw new SchemaFormatException(
                            $"Invalid token in request queue name description at path {tkn.Path}", ex);
                    }
                }

                

                var autoDelete = true;
                if (job.TryGetValue("autoDelete", StringComparison.InvariantCultureIgnoreCase, out tkn))
                {
                    try
                    {
                        autoDelete = tkn.ToObject<bool>();
                    }
                    catch (Exception ex)
                    {
                        throw new SchemaFormatException(
                            $"Invalid token in request queue auto delete description at path {tkn.Path}", ex);
                    }
                }

               

                var durable = false;
                if (job.TryGetValue("durable", StringComparison.InvariantCultureIgnoreCase, out tkn))
                {
                    try
                    {
                        durable = tkn.ToObject<bool>();
                    }
                    catch (Exception ex)
                    {
                        throw new SchemaFormatException(
                            $"Invalid token in request queue durable description at path {tkn.Path}", ex);
                    }
                }

                

                return new RequestQueueSchema(name, durable, autoDelete);

            }
            throw new SchemaFormatException($"Invalid token in exchange description at path {token.Path}");
        }

        
        private static TypeSchemaGreen GetFieldType(string typeName, IDictionary<string, TypeSchemaGreen> known)
        {
            if (known.TryGetValue(typeName, out var res))
                return res;
            if (WellKnownTypeSchemaGreen.ByCode.TryGetValue(typeName, out var wkt))
            {
                known.Add(wkt.SchemaName, wkt);
                return wkt;
            }
            if (typeName.EndsWith("[]"))
            {
                var at = new ArrayTypeSchemaGreen(Option.None, -1);
                known.Add(typeName, at);
                var et = GetFieldType(typeName.Substring(0, typeName.Length - 2), known);
                at = new ArrayTypeSchemaGreen(at, Option.None, et.Id);
                known[typeName] = at;
                return at;
            }
            if (typeName.EndsWith("?"))
            {
                var ot = new NullableTypeSchemaGreen(Option.None, -1);
                known.Add(typeName, ot);
                var et = GetFieldType(typeName.Substring(0, typeName.Length - 1), known);
                ot = new NullableTypeSchemaGreen(ot, Option.None, et.Id);
                known[typeName] = ot;
                return ot;
            }

            var rt = new RefTypeSchemaGreen();
            known.Add(typeName, rt);
            return rt;

        }

        private static TypeSchemaGreen JsonToType(JProperty typeProp, IDictionary<string, TypeSchemaGreen> known)
        {
            if (!(typeProp.Value is JObject jobj))
                throw new SchemaFormatException($"Invalid type description as {typeProp.Path}");

            if (!known.TryGetValue(typeProp.Name, out var type))
            {
                type = new RefTypeSchemaGreen();
                known.Add(typeProp.Name, type);
            }
            var codeName = (string) null;
            if (jobj.TryGetValue("title", StringComparison.InvariantCultureIgnoreCase, out var tkn))
            {
                try
                {
                    codeName = tkn.ToObject<string>();
                }
                catch (Exception ex)
                {
                    throw new SchemaFormatException(
                        $"Invalid token in codeName description at path {tkn.Path}", ex);
                }
            }

            var contract = (string) null;
            if (jobj.TryGetValue("contract", StringComparison.InvariantCultureIgnoreCase, out tkn))
            {
                try
                {
                    contract = tkn.ToObject<string>();
                }
                catch (Exception ex)
                {
                    throw new SchemaFormatException(
                        $"Invalid token in contract description at path {tkn.Path}", ex);
                }
            }

            if (jobj.TryGetValue("fields", StringComparison.InvariantCultureIgnoreCase, out var fieldToken))
            {
                if (!(fieldToken is JObject fields))
                    throw new SchemaFormatException($"Invalid fields description as path {fieldToken.Path}");

                TypeSchemaGreen @base = null;
                if (jobj.TryGetValue("base", StringComparison.InvariantCultureIgnoreCase, out tkn))
                {
                    try
                    {
                        var baseName = tkn.ToObject<string>();
                        if (!known.TryGetValue(baseName, out @base))
                        {
                            @base = new ComplexTypeSchemaGreen(Option.None, baseName, null, null, null, false,
                                ImmutableDictionary<string, int>.Empty);
                            known.Add(baseName, @base);
                        }

                    }
                    catch (Exception ex)
                    {
                        throw new SchemaFormatException(
                            $"Invalid token in base type description at path {tkn.Path}", ex);
                    }
                }

                bool isStruct = false;
                if (jobj.TryGetValue("struct", StringComparison.InvariantCultureIgnoreCase, out tkn))
                {
                    try
                    {
                        isStruct = tkn.ToObject<bool>();
                    }
                    catch (Exception ex)
                    {
                        throw new SchemaFormatException(
                            $"Invalid token in struct type description at path {tkn.Path}", ex);
                    }
                }

                var flds = new Dictionary<string, int>();

                foreach (var property in fields.Properties())
                {
                    var fieldName = property.Name;

                    try
                    {
                        var fieldTypeName = property.Value.ToObject<string>();
                        var fieldType = GetFieldType(fieldTypeName, known);
                        flds.Add(fieldName, fieldType.Id);

                    }
                    catch (Exception ex)
                    {
                        throw new SchemaFormatException(
                            $"Invalid token in type field description at path {property.Path}", ex);
                    }
                }

                type = new ComplexTypeSchemaGreen(type, Option.None, typeProp.Name, codeName, contract, @base?.Id,
                    isStruct,
                    ImmutableDictionary.CreateRange(flds));
                known[typeProp.Name] = type;
                return type;
            }
            
            if (jobj.TryGetValue("values", StringComparison.InvariantCultureIgnoreCase, out var valuesToken))
            {
                if (!(valuesToken is JObject values))
                    throw new SchemaFormatException($"Invalid values description as path {fieldToken.Path}");

                TypeSchemaGreen @base = WellKnownTypeSchemaGreen.ByType[typeof(int)];
                if (jobj.TryGetValue("base", StringComparison.InvariantCultureIgnoreCase, out tkn))
                {
                    try
                    {
                        var baseName = tkn.ToObject<string>();
                        if (!known.TryGetValue(baseName, out @base))
                        {
                            @base = WellKnownTypeSchemaGreen.ByCode[baseName];
                            known.Add(baseName, @base);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new SchemaFormatException(
                            $"Invalid token in based on description at path {tkn.Path}", ex);
                    }
                }

                bool isFlags = false;
                if (jobj.TryGetValue("flags", StringComparison.InvariantCultureIgnoreCase, out tkn))
                {
                    try
                    {
                        isFlags = tkn.ToObject<bool>();
                    }
                    catch (Exception ex)
                    {
                        throw new SchemaFormatException(
                            $"Invalid token in flags type description at path {tkn.Path}", ex);
                    }
                }

                var vls = new Dictionary<string, long>();

                foreach (var property in values.Properties())
                {
                    var fieldName = property.Name;

                    try
                    {
                        var fieldValue = property.Value.ToObject<long>();
                        vls.Add(fieldName, fieldValue);

                    }
                    catch (Exception ex)
                    {
                        throw new SchemaFormatException(
                            $"Invalid token in type field description at path {property.Path}", ex);
                    }
                }

                type = new EnumTypeSchemaGreen(type, Option.None, typeProp.Name, codeName, contract, @base.Id, isFlags, 
                    ImmutableDictionary.CreateRange(vls));
                known[typeProp.Name] = type;
                return type;
            }
            
            throw new SchemaFormatException($"Unknown type schema at {typeProp.Path}");
        }

        private static EventSchemaGreen JsonToEvent(JProperty evProp, IDictionary<string, TypeSchemaGreen> known)
        {
            if(!(evProp.Value is JObject jobj))
                throw new SchemaFormatException($"Invalid event description at path {evProp.Path}");
            TypeSchemaGreen contract;
            if (jobj.TryGetValue("contract", out var tkn))
            {
                try
                {
                    contract = GetFieldType(tkn.ToObject<string>(), known);
                }
                catch (Exception ex)
                {
                    throw new SchemaFormatException($"Invalid event contract found at path {tkn.Path}", ex);
                }
            }
            else
                throw new SchemaFormatException($"No event contract found at path {evProp.Path}");
            
            var codeName = (string) null;
            if (jobj.TryGetValue("title", StringComparison.InvariantCultureIgnoreCase, out tkn))
            {
                try
                {
                    codeName = tkn.ToObject<string>();
                }
                catch (Exception ex)
                {
                    throw new SchemaFormatException(
                        $"Invalid token in codeName description at path {tkn.Path}", ex);
                }
            }
            ExchangeSchema exchange = null;
            if (jobj.TryGetValue("exchange", out tkn))
                exchange = JsonToExchange(tkn);
            var routingKey = (string) null;
            if (jobj.TryGetValue("routingKey", StringComparison.InvariantCultureIgnoreCase, out tkn))
            {
                try
                {
                    routingKey = tkn.ToObject<string>();
                }
                catch (Exception ex)
                {
                    throw new SchemaFormatException(
                        $"Invalid token in routing description at path {tkn.Path}", ex);
                }
            }
            
            ContentType contentType = null;
            if (jobj.TryGetValue("contentType", StringComparison.InvariantCultureIgnoreCase, out tkn))
            {
                try
                {
                    contentType = new ContentType(tkn.ToObject<string>());
                }
                catch (Exception ex)
                {
                    throw new SchemaFormatException(
                        $"Invalid token in content type description at path {tkn.Path}", ex);
                }
            }
            
            return new EventSchemaGreen(evProp.Name, codeName, contract.Id, contentType, routingKey, exchange);
            
        }


        private static CallSchemaGreen JsonToCall(JProperty evProp, IDictionary<string, TypeSchemaGreen> known)
        {
            if(!(evProp.Value is JObject jobj))
                throw new SchemaFormatException($"Invalid call description at path {evProp.Path}");
            TypeSchemaGreen requestType;
            if (jobj.TryGetValue("request", out var tkn))
            {
                try
                {
                    requestType = GetFieldType(tkn.ToObject<string>(), known);
                }
                catch (Exception ex)
                {
                    throw new SchemaFormatException($"Invalid call request type found at path {tkn.Path}", ex);
                }
            }
            else
                throw new SchemaFormatException($"No call request type found at path {evProp.Path}");
            
            TypeSchemaGreen responseType = null;
            if (jobj.TryGetValue("response", out tkn))
            {
                try
                {
                    responseType = GetFieldType(tkn.ToObject<string>(), known);
                }
                catch (Exception ex)
                {
                    throw new SchemaFormatException($"Invalid call request type found at path {tkn.Path}", ex);
                }
            }
            
            
            
            var codeName = (string) null;
            if (jobj.TryGetValue("title", StringComparison.InvariantCultureIgnoreCase, out tkn))
            {
                try
                {
                    codeName = tkn.ToObject<string>();
                }
                catch (Exception ex)
                {
                    throw new SchemaFormatException(
                        $"Invalid token in codeName description at path {tkn.Path}", ex);
                }
            }
            ExchangeSchema exchange = null;
            if (jobj.TryGetValue("exchange", out tkn))
                exchange = JsonToExchange(tkn);
            
            ExchangeSchema responseExchange = null;
            if (jobj.TryGetValue("responseExchange", out tkn))
                responseExchange = JsonToExchange(tkn);
            
            var routingKey = (string) null;
            if (jobj.TryGetValue("routingKey", StringComparison.InvariantCultureIgnoreCase, out tkn))
            {
                try
                {
                    routingKey = tkn.ToObject<string>();
                }
                catch (Exception ex)
                {
                    throw new SchemaFormatException(
                        $"Invalid token in routing description at path {tkn.Path}", ex);
                }
            }
            
            
            
            RequestQueueSchema queue = null;
            if (jobj.TryGetValue("queue", out tkn))
                queue = JsonToRequestQueue(tkn);
            
            
            ContentType contentType = null;
            if (jobj.TryGetValue("contentType", StringComparison.InvariantCultureIgnoreCase, out tkn))
            {
                try
                {
                    contentType = new ContentType(tkn.ToObject<string>());
                }
                catch (Exception ex)
                {
                    throw new SchemaFormatException(
                        $"Invalid token in content type description at path {tkn.Path}", ex);
                }
            }
            
            return new CallSchemaGreen(evProp.Name, codeName, requestType.Id,
                responseType?.Id, contentType, routingKey, queue, exchange, responseExchange);
            
        }
        

        public static ServiceSchema FromJObject(JToken token)
        {
            if(!(token is JObject jobj))
                throw new SchemaFormatException("Invalid service schema");
            string name;
            if (jobj.TryGetValue("name", StringComparison.InvariantCultureIgnoreCase, out var tkn))
            {
                try
                {
                    name = tkn.ToObject<string>();
                }
                catch (Exception ex)
                {
                    throw new SchemaFormatException(
                        $"Invalid token in service name at path {tkn.Path}", ex);
                }
            }
            else
                throw new SchemaFormatException("No service name specified");
            
            string owner;
            if (jobj.TryGetValue("owner", StringComparison.InvariantCultureIgnoreCase, out tkn))
            {
                try
                {
                    owner = tkn.ToObject<string>();
                }
                catch (Exception ex)
                {
                    throw new SchemaFormatException(
                        $"Invalid token in service owner at path {tkn.Path}", ex);
                }
            }
            else
                throw new SchemaFormatException("No service owner specified");
            
            var codeName = (string) null;
            if (jobj.TryGetValue("title", StringComparison.InvariantCultureIgnoreCase, out tkn))
            {
                try
                {
                    codeName = tkn.ToObject<string>();
                }
                catch (Exception ex)
                {
                    throw new SchemaFormatException(
                        $"Invalid token in codeName description at path {tkn.Path}", ex);
                }
            }
            
            ContentType contentType = null;
            if (jobj.TryGetValue("contentType", StringComparison.InvariantCultureIgnoreCase, out tkn))
            {
                try
                {
                    contentType = new ContentType(tkn.ToObject<string>());
                }
                catch (Exception ex)
                {
                    throw new SchemaFormatException(
                        $"Invalid token in content type description at path {tkn.Path}", ex);
                }
            }
            
            ExchangeSchema exchange = null;
            if (jobj.TryGetValue("exchange", out tkn))
                exchange = JsonToExchange(tkn);
            
            ExchangeSchema responseExchange = null;
            if (jobj.TryGetValue("responseExchange", out tkn))
                responseExchange = JsonToExchange(tkn);
            
            
            var known = new Dictionary<string, TypeSchemaGreen>();
            var events = new List<EventSchemaGreen>();
            if(jobj.TryGetValue("events", out tkn))
            {
                if (tkn is JObject obj)
                {
                    foreach (var property in obj.Properties())
                    {
                        events.Add(JsonToEvent(property, known));
                    }
                }       
                    
            }
            var calls = new List<CallSchemaGreen>();
            if(jobj.TryGetValue("calls", out tkn))
            {
                if (tkn is JObject obj)
                {
                    foreach (var property in obj.Properties())
                    {
                        calls.Add(JsonToCall(property, known));
                    }
                }       
                    
            }
            
            if(jobj.TryGetValue("enums", out tkn))
            {
                if (tkn is JObject obj)
                {
                    foreach (var property in obj.Properties())
                    {
                        JsonToType(property, known);
                    }
                }       
                    
            }
            
            if(jobj.TryGetValue("types", out tkn))
            {
                if (tkn is JObject obj)
                {
                    foreach (var property in obj.Properties())
                    {
                        JsonToType(property, known);
                    }
                }       
                    
            }
            
            return new ServiceSchema(new ServiceSchemaGreen(name, owner, codeName,
                ImmutableDictionary.CreateRange(events.Select(p => new KeyValuePair<string, EventSchemaGreen>(p.Name, p))),
                ImmutableDictionary.CreateRange(calls.Select(p => new KeyValuePair<string, CallSchemaGreen>(p.Name, p))),
                ImmutableDictionary.CreateRange(known.Select(p => new KeyValuePair<int, TypeSchemaGreen>(p.Value.Id, p.Value))),
                contentType, exchange, responseExchange));
        }
        
        public JObject ToJObject()
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
                    obj.Add("autoDelete", true);
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

            JObject EventToJson(EventSchema eventSchema)
            {
                var obj = new JObject
                {
                    {"contract", eventSchema.EventType.SchemaName}
                };
                if (!string.IsNullOrWhiteSpace(eventSchema.CodeName))
                    obj["title"] = eventSchema.CodeName;
                
                if(eventSchema.HasExchange)
                    ExchangeToJson(eventSchema.Exchange).IfSome(p => obj.Add("exchange", p));
                if(eventSchema.HasRoutingKey)
                    obj.Add("routingKey", eventSchema.RoutingKey);
                if(eventSchema.HasContentType)
                    obj.Add("contentType", eventSchema.ContentType.ToString());
                return obj;
            }
            
            JObject CallToJson(CallSchema callSchema)
            {
                var obj = new JObject
                {
                    {"request", callSchema.RequestType.SchemaName}
                };
                if (!string.IsNullOrWhiteSpace(callSchema.CodeName))
                    obj["title"] = callSchema.CodeName;
                
                
                if (callSchema.ResponseType != null)
                    obj.Add("response", callSchema.ResponseType.SchemaName);
                
                if(callSchema.HasExchange)
                    ExchangeToJson(callSchema.Exchange).IfSome(p => obj.Add("exchange", p));
                if(callSchema.HasResponseExchange)
                    ExchangeToJson(callSchema.ResponseExchange).IfSome(p => obj.Add("responseExchange", p));
                if(callSchema.HasRoutingKey)
                    obj.Add("routingKey", callSchema.RoutingKey);
                if(callSchema.HasContentType)
                    obj.Add("contentType", callSchema.ContentType.ToString());
                if(callSchema.HasRequestQueue)
                    RequestQueueToJson(callSchema.RequestQueue).IfSome(p => obj.Add("queue", p));
                return obj;
            }

            JObject TypeToJson(ITypeSchema desc)
            {
                var obj = new JObject();
                if (desc.CodeName != null)
                    obj.Add("title", desc.CodeName);
                if(desc.ContractName != null && desc.CodeName != desc.SchemaName)
                    obj.Add("contract", desc.ContractName);
                switch (desc)
                {
                    case IComplexTypeSchema complexTypeDesc:
                        
                        if (complexTypeDesc.BaseOn != null)
                            obj.Add("base", complexTypeDesc.BaseOn.SchemaName);
                        if (complexTypeDesc.IsStruct)
                            obj.Add("struct", true);
                        obj.Add("fields", new JObject(complexTypeDesc.Fields.Select(p => new JProperty(p.Key, p.Value.SchemaName))));
                        return obj;
                    case IEnumTypeSchema enumTypeDesc:
                        obj.Add("base", enumTypeDesc.BaseOn.SchemaName);
                        if (enumTypeDesc.IsFlags)
                            obj.Add("flags", true);
                        obj.Add("values", new JObject(enumTypeDesc.Values.Select(p => new JProperty(p.Key, p.Value))));
                        return obj;
                    default:
                        throw new SchemaException("Unknown type description");
                }
            }

            var jsonSchema = new JObject
            {
                {"name", _schema.Name}, 
                {"owner", _schema.Owner}
            };
            if (!string.IsNullOrWhiteSpace(_schema.CodeName))
                jsonSchema["title"] = _schema.CodeName;
            if(_schema.HasContentType)
                jsonSchema.Add("content", _schema.ContentType.ToString());
            if (_schema.HasExchange)
                ExchangeToJson(_schema.Exchange).IfSome(p => jsonSchema.Add("exchange", p));
            if (_schema.HasResponseExchange)
                ExchangeToJson(_schema.ResponseExchange).IfSome(p => jsonSchema.Add("responseExchange", p));
            jsonSchema.Add("events", new JObject(_schema.Events.Select(p => new JProperty(p.Name, EventToJson(p))).Cast<object>().ToArray()));
            jsonSchema.Add("calls", new JObject(_schema.Calls.Select(p => new JProperty(p.Name, CallToJson(p))).Cast<object>().ToArray()));
            jsonSchema.Add("enums", new JObject(_schema.Types.OfType<IEnumTypeSchema>().Select(p => new JProperty(p.SchemaName, TypeToJson(p))).Cast<object>().ToArray()));
            jsonSchema.Add("types", new JObject(_schema.Types.OfType<IComplexTypeSchema>().Select(p => new JProperty(p.SchemaName, TypeToJson(p))).Cast<object>().ToArray()));
            return jsonSchema;

        }
    }
}