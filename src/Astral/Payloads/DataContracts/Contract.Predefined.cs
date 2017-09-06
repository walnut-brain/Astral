using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using FunEx;


namespace Astral.Payloads.DataContracts
{
    public static partial class Contract
    {
        public static readonly ComplexTypeToContract ArrayLikeTypeMapper =
            (type, resolver) =>
            {
                var elementType = TryGetElementType(type);

                return elementType
                    .ToResult(new ContractResolutionException("Cannot deteminate element type"))
                    .Bind(p => resolver(p))
                    .Map(p => $"{p}[]");
            };

        public static readonly TypeToContract AttributeTypeMapper =
            type =>
            {
                var attr = type.GetCustomAttribute<ContractAttribute>();
                if (attr != null)
                    return attr.Name;
                return new TypeToContractException(type);
            };

        public static ComplexTypeToContract DefaultTypeMapper(WellKnownTypes wellKnownTypes) =>
            WellKnownTypeMapper(wellKnownTypes).Fallback(ArrayLikeTypeMapper).Fallback(AttributeTypeMapper);

        public static readonly ComplexContractToType ArrayContractMapper =
            (contract, awaited, resolver) =>
            {
                if (!contract.EndsWith("[]")) return new ContractToTypeException(contract).ToFail<Type>();
                var elementName = contract.Remove(contract.Length - 2);

                var elementTypes = 
                    awaited
                        .SelectMany(p => TryGetElementType(p).Match(t => t.AsEnumerable(), () => Enumerable.Empty<Type>()))
                        .ToImmutableList();

                return resolver(elementName, elementTypes).Map(p => p.MakeArrayType());
            };

        public static readonly ComplexContractToType AttributeContractMapper =
            (contract, awaited, resolver) =>
            {
                return awaited.SelectMany(
                    at =>
                    {
                        if (CheckSubtype(at)) return at.AsEnumerable();
                        var attrs = at.GetCustomAttributes<KnownTypeAttribute>();
                        var nt =
                            attrs.SelectMany(known =>
                            {
                                if (known.MethodName == null) return new[] {known.Type};
                                var method = at.GetMethod(known.MethodName);
                                return (IEnumerable<Type>) method.Invoke(null, new object[0]);
                            }).ToImmutableList();
                        return resolver(contract, nt).Map(ImmutableList.Create).RecoverTo(ImmutableList<Type>.Empty);
                    }).FirstOrNone().ToResult(new ContractToTypeException(contract));

                bool CheckSubtype(Type t)
                {
                    var a1 = t.GetCustomAttribute<ContractAttribute>();
                    if (a1 != null)
                        return a1.Name == contract;
                    return false;
                }
            };

        public static ComplexContractToType DefaultContractMapper(WellKnownTypes wellKnownTypes) =>
            WellKnownContractMapper(wellKnownTypes).Fallback(ArrayContractMapper).Fallback(AttributeContractMapper);

        public static TypeToContract WellKnownTypeMapper(WellKnownTypes knowns) =>
            type =>
            {
                if (knowns.IsUnit(type))
                    return WellKnownTypes.UnitCode;
                if (knowns.TryGetCode(type, out var code))
                    return code;
                return new TypeToContractException(type);
            };


        public static ContractToType WellKnownContractMapper(WellKnownTypes knowns) =>
            (contract, awaited) =>
            {
                if (contract == WellKnownTypes.UnitCode)
                    return knowns.DefaultUnitType;
                if (knowns.TryGetType(contract, out var type))
                    return type;
                return new ContractToTypeException(contract);
            };


        private static Option<Type> TryGetElementType(Type arrayLikeType)
        {
            if (arrayLikeType.IsArray)
            {
                var elementType = arrayLikeType.GetElementType();
                return elementType.ToOption();
            }
            var enumIntf = arrayLikeType.GetInterfaces()
                .FirstOrDefault(p => p.IsGenericType && p.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            if (enumIntf != null)
            {
                var elementType = enumIntf.GetGenericArguments()[0];
                return elementType.ToOption();
            }
            return Option.None;
        }
    }
}