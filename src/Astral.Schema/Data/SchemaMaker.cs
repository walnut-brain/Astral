using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Astral.Markup;

namespace Astral.Schema.Data
{
    public static class SchemaMaker
    {
        public static IEnumerable<TypeDesc> FromTypeList(ICollection<(Type[], Action<string[]>)> types, bool contractFromName)
        {
            var knownMaps = new Dictionary<Type, TypeDesc>();
            var toProcess = new Stack<Type>();
            foreach (var type in types.Select(p => p.Item1).SelectMany(p => p))
                toProcess.Push(type);
            while (toProcess.Count > 0)
            {
                var type = toProcess.Pop();
                ProcessType(type, null, contractFromName, knownMaps);
            }

            var duplicate = knownMaps.Values.OfType<NamedTypeDesc>().GroupBy(p => p.Name).Select(p => (p.Key, p.Count())).Where(p => p.Item2 > 1).Select(p => p.Item1).FirstOrDefault();
            if(duplicate != null)
                throw new SchemaException($"Duplicate type name {duplicate}");

            foreach (var (typeToCall, action) in types)
            {
                var desc =  typeToCall.Select(p => knownMaps[p]).ToArray();
                action(desc.Select(p => p.Contract).ToArray());
            }
            return knownMaps.Values.Where(p => p is NamedTypeDesc);

        }

        private static TypeDesc ProcessType(Type type, string contractName, bool contractFromName, Dictionary<Type, TypeDesc> known)
        {
            if (known.TryGetValue(type, out var found))
                return found;
            var simple = SimpleTypeDesc.FromType(type);
            if (simple.IsSome)
            {
                known.Add(type, simple.Unwrap());
                return simple.Unwrap();
            }
            if (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var desc = new NullableTypeDesc(type, ProcessType(type.GenericTypeArguments[0], null, contractFromName, known));
                known.Add(type, desc);
                return desc;
            }
            if (type.IsArray)
            {
                var element = type.GetElementType();
                var desc  = new ArrayTypeDesc(ProcessType(element, null, contractFromName, known), type);
                known.Add(type, desc);
                return desc;
            }
            var enumerable = type.GetInterfaces().Where(p => p.IsConstructedGenericType)
                        .Select(p => (p,  p.GetGenericTypeDefinition())).Where(p => p.Item2 == typeof(IEnumerable<>)).Select(p => p.Item1).FirstOrDefault();
            if (enumerable != null)
            {
                var element = enumerable.GenericTypeArguments[0];
                var desc  = new ArrayTypeDesc(ProcessType(element, null, contractFromName, known), type);
                known.Add(type, desc);
                return desc;
            }
            
            if (type.IsEnum)
            {
                var baseType = (SimpleTypeDesc) ProcessType(Enum.GetUnderlyingType(type), null, contractFromName, known);
                var contract = contractName ?? type.GetCustomAttribute<ContractAttribute>()?.Name ??
                               (contractFromName ? ContractFromName(type.Name) : null);
                var enumType = new EnumTypeDesc(type, NormalizeName(type), contract, baseType, Enum.GetValues(type).Cast<object>().Select(p => (p, Convert.ToInt64(p))).Select(p => new KeyValuePair<string, long>(Enum.GetName(type, p.Item1), p.Item2)));
                known.Add(type, enumType);
                return enumType;
            }
            if (type.IsClass)
            {
                ComplexTypeDesc baseDesc = null;
                var baseType = type.BaseType;
                if (baseType != typeof(object))
                    baseDesc = (ComplexTypeDesc) ProcessType(baseType, null, contractFromName, known);
                ComplexTypeDesc typeDesc;
                var contract = contractName ?? type.GetCustomAttribute<ContractAttribute>()?.Name ??
                               (contractFromName ? ContractFromName(type.Name) : null);
                var propQuery = type.GetProperties().Where(p => p.DeclaringType == type)
                    .Select(p => (p.Name, ProcessType(p.PropertyType, null, contractFromName, known)));
                if (baseDesc != null)
                    typeDesc = new ComplexTypeDesc(type, NormalizeName(type), contract, baseDesc,
                        propQuery.Select(p => new KeyValuePair<string, TypeDesc>(p.Item1, p.Item2)));
                else
                    typeDesc = new ComplexTypeDesc(type, NormalizeName(type), contract, propQuery.ToDictionary(p => p.Item1, p => p.Item2));
                    
                known.Add(type, typeDesc);
                foreach (var attribute in type.GetCustomAttributes<KnownContractAttribute>())
                {
                    ProcessType(attribute.Type, attribute.Contract, contractFromName, known);
                }
                return typeDesc;
            }
            if (type.IsValueType)
            {
                
                var contract = type.GetCustomAttribute<ContractAttribute>()?.Name ??
                               (contractFromName ? ContractFromName(type.Name) : null);
                var propQuery = type.GetProperties()
                    .Select(p => (p.Name, ProcessType(p.PropertyType, null, contractFromName, known)));
                var typeDesc = new ComplexTypeDesc(type, NormalizeName(type), contract, propQuery.ToDictionary(p => p.Item1, p => p.Item2));
                known.Add(type, typeDesc);
                return typeDesc;
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