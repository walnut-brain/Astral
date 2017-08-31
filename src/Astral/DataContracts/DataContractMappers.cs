using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Astral.Core;
using Astral.Exceptions;
using Astral.Markup;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Astral.DataContracts
{

    public static class DataContractMappers
    {
        public static Option<string> WellKnownTypeMapper(Type type)
        {
            if (WellKnownTypes.UnitTypes.Any(p => p == type))
                return WellKnownTypes.UnitTypeCode;
            if (WellKnownTypes.CodeByType.TryGetValue(type, out var code))
                return code;
            return None;
        }

        public static Option<string> ArrayLikeMapper(Type type, ITypeToContract elementMapper)
        {
            var elementType = TryGetElementType(type);

            return elementType.Bind(elementMapper.TryMap).Map(p => $"{p}[]");
        }

        public static Option<string> AttributeMapper(Type type)
        {
            var attr = type.GetCustomAttribute<ContractAttribute>();
            if (attr != null)
                return attr.Name;
            return None;
        }

        public static Option<string> DefaultTypeMapper(Type type, ITypeToContract elementMapper)
            => DataContract.OrElse(WellKnownTypeMapper, ArrayLikeMapper).OrElse(AttributeMapper)(type, elementMapper);

        internal static Option<Type> TryGetElementType(Type arrayLikeType)
        {
            if (arrayLikeType.IsArray)
            {
                var elementType = arrayLikeType.GetElementType();
                return elementType;
            }
            var enumIntf = arrayLikeType.GetInterfaces()
                .FirstOrDefault(p => p.IsGenericType && p.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            if (enumIntf != null)
            {
                var elementType = enumIntf.GetGenericArguments()[0];
                return elementType;
            }
            return None;
        }
    
        public static Option<Type> WellKnownContractMapper(string contract)
        {
            if (contract == WellKnownTypes.UnitTypeCode)
                return WellKnownTypes.UnitTypes[0];
            if (WellKnownTypes.TypeByCode.TryGetValue(contract, out var type))
                return type;
            return None;
        }

        public static Option<Type> ArrayContractMapper(string contract, Seq<Type> awaited, IContractToType ctt)
        {
            if (!contract.EndsWith("[]")) return None;
            var elementName = contract.Remove(contract.Length - 2);

            var elementTypes = awaited.Bind(p => TryGetElementType(p).Match(t => t.Cons(), Seq<Type>));

            return ctt.TryMap(elementName, elementTypes).Map(p => p.MakeArrayType());
        }

        public static Option<Type> AttributeContractMapper(string contract, Seq<Type> awaited)
        {
            return awaited.Bind(
                at =>
                {
                    if (CheckSubtype(at)) return at.Cons();
                    var attrs = at.GetCustomAttributes<KnownTypeAttribute>();
                    foreach (var known in attrs)
                        if (known.MethodName != null)
                        {
                            var method = at.GetMethod(known.MethodName);
                            var subTypes = (IEnumerable<Type>) method.Invoke(null, new object[0]);
                            var found = subTypes.FirstOrDefault(CheckSubtype);
                            if (found != null) found.Cons();
                        }
                        else if (CheckSubtype(known.Type))
                        {
                            return known.Type.Cons();
                        }
                    return Seq<Type>();
                }).HeadOrNone();

            bool CheckSubtype(Type t)
            {
                var a1 = t.GetCustomAttribute<ContractAttribute>();
                if (a1 != null)
                    return a1.Name == contract;
                return false;
            }
        }

        public static Option<Type> DefaultContractMapper(string contract, Seq<Type> awaited,
            IContractToType elementMapper)
            => DataContract.OrElse(WellKnownContractMapper, ArrayContractMapper).OrElse(AttributeContractMapper)(
                contract, awaited, elementMapper);
    }

    
}