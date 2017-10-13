using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using Astral.Markup;
using Astral.Schema.Green;

namespace Astral.Schema.Data
{
    public static class SchemaMaker
    {
        public static ImmutableDictionary<int, TypeSchemaGreen> FromTypeList(ICollection<(Type[], Action<int[]>)> types, bool contractFromName)
        {
            var knownMaps = new Dictionary<Type, TypeSchemaGreen>();
            var toProcess = new Stack<Type>();
            foreach (var type in types.Select(p => p.Item1).SelectMany(p => p))
                toProcess.Push(type);
            while (toProcess.Count > 0)
            {
                var type = toProcess.Pop();
                ProcessType(type, null, contractFromName, knownMaps);
            }

            var duplicate = knownMaps.Values
                .Where(p => !p.IsWellKnown).OfType<IHasSchemaName>().GroupBy(p => p.SchemaName).Select(p => (p.Key, p.Count())).Where(p => p.Item2 > 1).Select(p => p.Item1).FirstOrDefault();
            if(duplicate != null)
                throw new SchemaException($"Duplicate type name {duplicate}");

            foreach (var (typeToCall, action) in types)
            {
                var desc =  typeToCall.Select(p => knownMaps[p]).ToArray();
                action(desc.Select(p => p.Id).ToArray());
            }
            return ImmutableDictionary.CreateRange(knownMaps.Values.Select(p => new KeyValuePair<int, TypeSchemaGreen>(p.Id, p)));

        }

        private static int ProcessType(Type type, string contractName, bool contractFromName, Dictionary<Type, TypeSchemaGreen> known)
        {
            if (known.TryGetValue(type, out var found))
                return found.Id;
            
            if (WellKnownTypeSchemaGreen.ByType.TryGetValue(type, out var wk))
            {
                known.Add(type, wk);
                return wk.Id;
            }
            if (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var desc = new NullableTypeSchemaGreen(type, -1);
                known.Add(type, desc);
                known[type] = new NullableTypeSchemaGreen(desc, type, ProcessType(type.GenericTypeArguments[0], null, contractFromName, known));
                return desc.Id;
            }
            if (type.IsArray)
            {
                var element = type.GetElementType();
                var desc  = new ArrayTypeSchemaGreen(type, -1);
                known.Add(type, desc);
                known[type] = new ArrayTypeSchemaGreen(desc, type, ProcessType(element, null, contractFromName, known));
                return desc.Id;
            }
            var enumerable = type.GetInterfaces().Where(p => p.IsConstructedGenericType)
                        .Select(p => (p,  p.GetGenericTypeDefinition())).Where(p => p.Item2 == typeof(IEnumerable<>)).Select(p => p.Item1).FirstOrDefault();
            if (enumerable != null)
            {
                var element = enumerable.GenericTypeArguments[0];
                var desc  = new ArrayTypeSchemaGreen(type, -1);
                known.Add(type, desc);
                known[type] = new ArrayTypeSchemaGreen(desc, type, ProcessType(element, null, contractFromName, known));
                return desc.Id;
            }

            if (type.IsEnum)
            {
                var baseTypeId = ProcessType(Enum.GetUnderlyingType(type), null, contractFromName, known);
                var contract = contractName ?? type.GetCustomAttribute<ContractAttribute>()?.Name ??
                               (contractFromName ? ContractFromName(type.Name) : null);
                var schemaName = type.GetCustomAttribute<SchemaNameAttribute>()?.Name ?? NormalizeName(type);
                var enumType =
                    new EnumTypeSchemaGreen(type, schemaName, NormalizeName(type), contract, baseTypeId,
                        type.GetCustomAttribute<FlagsAttribute>() != null,
                        ImmutableDictionary.CreateRange(
                            Enum.GetValues(type).Cast<object>().Select(p => (p, Convert.ToInt64(p))).Select(p =>
                                new KeyValuePair<string, long>(Enum.GetName(type, p.Item1), p.Item2))));
                known.Add(type, enumType);
                return enumType.Id;
            }
            if (type.IsClass)
            {
                var contract = contractName ?? type.GetCustomAttribute<ContractAttribute>()?.Name ??
                               (contractFromName ? ContractFromName(type.Name) : null);
                var schemaName = type.GetCustomAttribute<SchemaNameAttribute>()?.Name ?? NormalizeName(type);
                var desc = new ComplexTypeSchemaGreen(type, schemaName, NormalizeName(type), contract, -1, false, ImmutableDictionary<string, int>.Empty);
                known.Add(type, desc);
                int? baseTypeId = null;
                var baseType = type.BaseType;
                if (baseType != typeof(object))
                    baseTypeId = ProcessType(baseType, null, contractFromName, known);
                
                var propQuery = type.GetProperties().Where(p => p.DeclaringType == type)
                    .Select(p => new KeyValuePair<string, int>(p.Name, ProcessType(p.PropertyType, null, contractFromName, known)));
                known[type] = new ComplexTypeSchemaGreen(desc, type, schemaName, NormalizeName(type), contract, baseTypeId, false,
                    ImmutableDictionary.CreateRange(propQuery));
                    
                foreach (var attribute in type.GetCustomAttributes<KnownContractAttribute>())
                {
                    ProcessType(attribute.Type, attribute.Contract, contractFromName, known);
                }
                return desc.Id;
            }
            if (type.IsValueType)
            {
                
                var contract = type.GetCustomAttribute<ContractAttribute>()?.Name ??
                               (contractFromName ? ContractFromName(type.Name) : null);
                var schemaName = type.GetCustomAttribute<SchemaNameAttribute>()?.Name ?? NormalizeName(type);
                var desc = new ComplexTypeSchemaGreen(type, schemaName, NormalizeName(type), contract, null, true, ImmutableDictionary<string, int>.Empty);
                known.Add(type, desc);
                
                
                var propQuery = type.GetProperties()
                    .Select(p => new KeyValuePair<string, int>(p.Name, ProcessType(p.PropertyType, null, contractFromName, known)));
                known[type] = new ComplexTypeSchemaGreen(desc, type, schemaName, NormalizeName(type), contract, null, true,
                    ImmutableDictionary.CreateRange(propQuery));
                return desc.Id;
            }
            
            
            throw new SchemaException($"Cannot create schema for type {type}");
        }

        private static string NormalizeName(Type type)
        {
            var fn = type.FullName;
            var dotPos = fn.LastIndexOf(".", StringComparison.InvariantCulture);
            if (dotPos >= 0)
                fn = fn.Substring(dotPos + 1);
            return fn.Replace("+", ".");
        }

        private static string ContractFromName(string name)
            => name.SelectMany((p, i) => i > 0 && char.IsUpper(p) ? new[] {'.', char.ToLower(p)} : new[] {p})
                .Aggregate(new StringBuilder(), (sb, ch) => sb.Append(ch), sb => sb.ToString());
    }
}