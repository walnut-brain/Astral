using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Astral.Markup.RabbitMq;
using Astral.Schema.Data;

namespace Astral.Schema.CSharpGenerator
{
    public class CSharpGenerator
    {
        public CSharpGenerator(string ns)
        {
            Namespace = ns;
        }

        public string Namespace { get; set; }

        private static Dictionary<Type, string> KnownTypeNames = new Dictionary<Type, string>()
        {
            { typeof(byte), "byte" },
            { typeof(sbyte), "sbyte" },
            { typeof(ushort), "ushort" },
            { typeof(short), "short" },
            { typeof(uint), "uint" },
            { typeof(int), "int" },
            { typeof(ulong), "ulong" },
            { typeof(long), "long" },
            { typeof(float), "float" },
            { typeof(double), "double" },
            { typeof(string), "string" },
            { typeof(Guid), "Guid" },
            { typeof(DateTime), "DateTime" },
            { typeof(DateTimeOffset), "DateTimeOffset" },
            { typeof(TimeSpan), "TimeSpan" }
        };
        
        public string Generate(ServiceSchema schema)
        {
            var writer = new IndentWriter("    ");
            writer.WriteLine("using System;");
            writer.WriteLine("using Astral.Markup;");
            writer.WriteLine("using Astral.Markup.RabbitMq;");
            writer.WriteLine();
            writer.WriteLine($"namespace {Namespace}");
            writer.WriteLine("{");
            using (writer.Indent())
            {
                writer.WriteLine($"[Service(\"{schema.Name}\")]");
                writer.WriteLine($"[Owner(\"{schema.Owner}\")]");
                if (schema.HasExchange)
                {
                    var builder = new StringBuilder();
                    var exchange = schema.Exchange;
                    builder.Append("[Exchange(");
                    WriteExchange(exchange, builder, false);
                    builder.Append(")]");
                    writer.WriteLine(builder.ToString());
                }
                if (schema.HasResponseExchange)
                {
                    var builder = new StringBuilder();
                    var exchange = schema.Exchange;
                    builder.Append("[ResponseExchange(");
                    WriteExchange(exchange, builder, true);
                    builder.Append(")]");
                    writer.WriteLine(builder.ToString());
                }
                
                if(schema.HasContentType)
                    writer.WriteLine($"[ContentType({schema.ContentType})]");
                
                writer.WriteLine("public interface " + schema.CodeName);
                writer.WriteLine("{");
                using (writer.Indent())
                {
                    foreach (var eventSchema in schema.Events)
                    {
                        writer.WriteLine($"[Endpoint(\"{eventSchema.Name}\")]");
                        if(eventSchema.HasContentType)
                            writer.WriteLine($"[ContentType({eventSchema.ContentType})]");
                        if(eventSchema.HasExchange)
                        {
                            var builder = new StringBuilder();
                            var exchange = eventSchema.Exchange;
                            builder.Append("[Exchange(");
                            WriteExchange(exchange, builder, false);
                            builder.Append(")]");
                            writer.WriteLine(builder.ToString());
                        }
                        if(eventSchema.HasRoutingKey)
                            writer.WriteLine($"[RoutingKey(\"{eventSchema.RoutingKey}\")]");
                        writer.WriteLine($"EventHandler<{ToTypeName(eventSchema.EventType)}> {eventSchema.CodeName} {{ get; }}"  );
                        writer.WriteLine();
                    }
                    
                    foreach (var callSchema in schema.Calls)
                    {
                        writer.WriteLine($"[Endpoint(\"{callSchema.Name}\")]");
                        if(callSchema.HasContentType)
                            writer.WriteLine($"[ContentType({callSchema.ContentType})]");
                        if(callSchema.HasExchange)
                        {
                            var builder = new StringBuilder();
                            var exchange = callSchema.Exchange;
                            builder.Append("[Exchange(");
                            WriteExchange(exchange, builder, false);
                            builder.Append(")]");
                            writer.WriteLine(builder.ToString());
                        }
                        if(callSchema.HasResponseExchange)
                        {
                            var builder = new StringBuilder();
                            var exchange = callSchema.ResponseExchange;
                            builder.Append("[ResponseExchange(");
                            WriteExchange(exchange, builder, true);
                            builder.Append(")]");
                            writer.WriteLine(builder.ToString());
                        }
                        if(callSchema.HasRoutingKey)
                            writer.WriteLine($"[RoutingKey(\"{callSchema.RoutingKey}\")]");
                        if (callSchema.HasRequestQueue)
                        {
                            var builder = new StringBuilder();
                            builder.Append("[RpcQueue(");
                            WriteQueue(callSchema.RequestQueue, builder);
                            builder.Append(")]");
                            writer.WriteLine(builder.ToString());
                        }
                        if (callSchema.ResponseType == null)
                        {
                            writer.WriteLine($"Action<{ToTypeName(callSchema.RequestType)}> {callSchema.CodeName} {{ get; }}");
                        }
                        else
                        {
                            writer.WriteLine($"Func<{ToTypeName(callSchema.RequestType)}, {ToTypeName(callSchema.ResponseType)}> {callSchema.CodeName} {{ get; }}");
                        }
                        
                        writer.WriteLine();
                    }
                }
                
                writer.WriteLine("}");

                var tp = schema.Types is ICollection<ITypeSchema> c ? c : schema.Types.ToList();
                foreach (var type in schema.Types.Where(p => !p.CodeName.Contains(".")))
                {
                    switch (type)
                    {
                        case IComplexTypeSchema complexTypeDeclarationSchema:
                            WriteComplex(complexTypeDeclarationSchema, writer, tp);
                            break;
                        case IEnumTypeSchema enumTypeDeclarationSchema:
                            WriteEnum(enumTypeDeclarationSchema, writer);
                            break;
                    }
                    
                }
            }
            writer.WriteLine("}");
            return writer.ToString();

        }

        private void WriteEnum(IEnumTypeSchema schema, IndentWriter writer)
        {
            writer.WriteLine();
            if(!string.IsNullOrWhiteSpace(schema.ContractName))
                writer.WriteLine($"[Contract(\"{schema.ContractName}\")]");
            if(!string.IsNullOrWhiteSpace(schema.SchemaName))
                writer.WriteLine($"[SchemaName(\"{schema.SchemaName}\")]");
            if(schema.IsFlags)
                writer.WriteLine("[Flags]");
            var lastDot = schema.CodeName.LastIndexOf(".", StringComparison.InvariantCulture);
            var name = lastDot < 0 ? schema.CodeName : schema.CodeName.Substring(lastDot + 1); 
            writer.WriteLine($"public enum {name}" + 
                             (schema.BaseOn.DotNetType != typeof(int) ? ": " + KnownTypeNames[schema.BaseOn.DotNetType] : ""));
            writer.WriteLine("{");
            using (writer.Indent())
            {
                foreach (var (nm, val, idx)  in schema.Values.Select((p, i) => (p.Key, p.Value, i)))
                {
                    if(idx < schema.Values.Count - 1)
                        writer.WriteLine($"{nm} = {val},");
                    else
                        writer.WriteLine($"{nm} = {val}");
                }
            }
            writer.WriteLine("}");
        }

        private void WriteComplex(IComplexTypeSchema schema, IndentWriter writer,
            ICollection<ITypeSchema> types)
        {
            writer.WriteLine();
            if (!string.IsNullOrWhiteSpace(schema.ContractName))
                writer.WriteLine($"[Contract(\"{schema.ContractName}\")]");
            if (!string.IsNullOrWhiteSpace(schema.SchemaName))
                writer.WriteLine($"[SchemaName(\"{schema.SchemaName}\")]");
            var lastDot = schema.CodeName.LastIndexOf(".", StringComparison.InvariantCulture);
            var name = lastDot < 0 ? schema.CodeName : schema.CodeName.Substring(lastDot + 1);
            
            string TrimName(string nm) =>
                nm.StartsWith(schema.CodeName) ? nm.Substring(schema.CodeName.Length + 1) : nm;
            if (schema.IsStruct)
            {
                writer.WriteLine($"public struct {name}");
                writer.WriteLine("{");
                using (writer.Indent())
                {
                    foreach (var field in schema.Fields)
                    {
                        writer.WriteLine($"public {TrimName(ToTypeName(field.Value))} {field.Key} {{ get;set; }}");
                    }

                    foreach (var subType in types.Where(p => p.CodeName.StartsWith(schema.CodeName)))
                    {
                        switch (subType)
                        {
                            case IComplexTypeSchema complexTypeDeclarationSchema:
                                WriteComplex(complexTypeDeclarationSchema, writer, types);
                                break;
                            case IEnumTypeSchema enumTypeDeclarationSchema:
                                WriteEnum(enumTypeDeclarationSchema, writer);
                                break;
                        }
                    }
                }
                writer.WriteLine("}");
            }
            else
            {
                foreach (var known in types.OfType<IComplexTypeSchema>().Where(p => p.BaseOn == schema))
                    if (!string.IsNullOrWhiteSpace(known.ContractName))
                        writer.WriteLine($"[KnownContract(typeof({known.CodeName}), \"{known.ContractName}\")]");
                    else
                        writer.WriteLine($"[KnownContract(typeof({known.CodeName}))]");
                if (schema.BaseOn != null)
                {

                    writer.WriteLine($"public class {name} : {TrimName(schema.BaseOn.CodeName)}");
                    writer.WriteLine("{");
                using (writer.Indent())
                {
                    foreach (var field in schema.Fields)
                    {
                        writer.WriteLine($"public {TrimName(ToTypeName(field.Value))} {field.Key} {{ get;set; }}");
                    }

                    foreach (var subType in types.Where(p => p.CodeName.StartsWith(schema.CodeName + ".") &&
                                                             !p.CodeName.Replace(schema.CodeName + ".", "").Contains(".")))
                    {
                        switch (subType)
                        {
                            case IComplexTypeSchema complexTypeDeclarationSchema:
                                WriteComplex(complexTypeDeclarationSchema, writer, types);
                                break;
                            case IEnumTypeSchema enumTypeDeclarationSchema:
                                WriteEnum(enumTypeDeclarationSchema, writer);
                                break;
                        }
                    }
                }
                writer.WriteLine("}");
                }
                else
                    writer.WriteLine($"public class {name}");
                writer.WriteLine("{");
                using (writer.Indent())
                {
                    foreach (var field in schema.Fields)
                    {
                        writer.WriteLine($"public {TrimName(ToTypeName(field.Value))} {field.Key} {{ get;set; }}");
                    }

                    foreach (var subType in types.Where(p => p.CodeName.StartsWith(schema.CodeName + ".") &&
                                                             !p.CodeName.Replace(schema.CodeName + ".", "").Contains(".")))
                    {
                        switch (subType)
                        {
                            case IComplexTypeSchema complexTypeDeclarationSchema:
                                WriteComplex(complexTypeDeclarationSchema, writer, types);
                                break;
                            case IEnumTypeSchema enumTypeDeclarationSchema:
                                WriteEnum(enumTypeDeclarationSchema, writer);
                                break;
                        }
                    }
                }
                writer.WriteLine("}");


            }

        }

        private string ToTypeName(ITypeSchema schema)
        {
            switch (schema)
            {
                case WellKnownTypeSchema wellKnownTypeSchema:
                    return KnownTypeNames[wellKnownTypeSchema.DotNetType];
                case IArrayTypeSchema arrayTypeDeclarationSchema:
                    return ToTypeName(arrayTypeDeclarationSchema.ElementType) + "[]";
                case IComplexTypeSchema complexTypeDeclarationSchema:
                    return complexTypeDeclarationSchema.CodeName;
                case IEnumTypeSchema enumTypeDeclarationSchema:
                    return enumTypeDeclarationSchema.CodeName;
                case IOptionTypeSchema optionTypeDeclarationSchema:
                    return ToTypeName(optionTypeDeclarationSchema.ElementType) + "?";
            }
            throw new ArgumentOutOfRangeException("Unknown type schema");
        }

        private static void WriteExchange(ExchangeSchema exchange, StringBuilder builder, bool response)
        {
            
            var notFirst = false;
            if (!string.IsNullOrWhiteSpace(exchange.Name) && !response ||
                exchange.Name != null && response)
            {
                builder.Append($"Name = \"{exchange.Name}\"");
                notFirst = true;
            }
            if (exchange.Type != ExchangeKind.Direct)
            {
                if (notFirst)
                    builder.Append(", ");
                builder.Append($"Kind = ExchangeKind.{exchange.Type}");
                notFirst = true;
            }
            if (!exchange.Durable)
            {
                if (notFirst)
                    builder.Append(", ");
                builder.Append("Durable = false");
                notFirst = true;
            }
            if (exchange.AutoDelete)
            {
                if (notFirst)
                    builder.Append(", ");
                builder.Append("AutoDelete = true");
                notFirst = true;
            }
            if (exchange.Delayed)
            {
                if (notFirst)
                    builder.Append(", ");
                builder.Append("Delayed = false");
                notFirst = true;
            }

            if (exchange.Alternate != null)
            {
                if (notFirst)
                    builder.Append(", ");
                builder.Append($"Alternate = \"{exchange.Alternate}\"");
            }
        }
        
        private static void WriteQueue(RequestQueueSchema queue, StringBuilder builder)
        {
            var notFirst = false;
            if (!string.IsNullOrWhiteSpace(queue.Name))
            {
                builder.Append($"Name = \"{queue.Name}\"");
                notFirst = true;
            }
            if (queue.Durable)
            {
                if (notFirst)
                    builder.Append(", ");
                builder.Append("Durable = true");
                notFirst = true;
            }
            if (!queue.AutoDelete)
            {
                if (notFirst)
                    builder.Append(", ");
                builder.Append("AutoDelete = false");
            }
            
        }
    }
}