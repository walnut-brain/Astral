using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using LanguageExt;

namespace Astral.Payloads.DataContracts
{
    public static partial class Contract
    {
        public static ComplexTypeToContract ArrayLikeTypeMapper =
            (type, resolver) =>
            {
                var elementType = TryGetElementType(type);

                return elementType
                    .ToTry(new ContractResolutionException("Cannot deteminate element type"))
                    .Bind(p => resolver(p))
                    .Map(p => $"{p}[]");
            };

        public static TypeToContract AttributeTypeMapper =
            type =>
            {
                var attr = type.GetCustomAttribute<ContractAttribute>();
                if (attr != null)
                    return Prelude.Try(attr.Name);
                return Prelude.Try<string>(new TypeToContractException(type));
            };

        public static ComplexTypeToContract DefaultTypeMapper(WellKnownTypes wellKnownTypes) =>
            WellKnownTypeMapper(wellKnownTypes).Fallback(ArrayLikeTypeMapper).Fallback(AttributeTypeMapper);

        public static ComplexContractToType ArrayContractMapper =
            (contract, awaited, resolver) =>
            {
                if (!contract.EndsWith("[]")) return Prelude.Try<Type>(new ContractToTypeException(contract));
                var elementName = contract.Remove(contract.Length - 2);

                var elementTypes = awaited.Bind(p => TryGetElementType(p).Match(t => t.Cons(), Prelude.Seq<Type>));

                return resolver(elementName, elementTypes).Map(p => p.MakeArrayType());
            };

        public static ComplexContractToType AttributeContractMapper =
            (contract, awaited, resolver) =>
            {
                return awaited.Bind(
                    at =>
                    {
                        if (CheckSubtype(at)) return at.Cons();
                        var attrs = at.GetCustomAttributes<KnownTypeAttribute>();
                        var nt =
                            attrs.SelectMany(known =>
                            {
                                if (known.MethodName == null) return new[] {known.Type};
                                var method = at.GetMethod(known.MethodName);
                                return (IEnumerable<Type>) method.Invoke(null, new object[0]);
                            }).ToSeq();
                        return resolver(contract, nt).Map(p => p.Cons()).IfFail(Prelude.Seq<Type>);
                    }).HeadOrNone().ToTry(new ContractToTypeException(contract));

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
                    return Prelude.Try(WellKnownTypes.UnitCode);
                if (knowns.TryGetCode(type, out var code))
                    return Prelude.Try(code);
                return Prelude.Try<string>(new TypeToContractException(type));
            };


        public static ContractToType WellKnownContractMapper(WellKnownTypes knowns) =>
            (contract, awaited) =>
            {
                if (contract == WellKnownTypes.UnitCode)
                    return Prelude.Try(knowns.DefaultUnitType);
                if (knowns.TryGetType(contract, out var type))
                    return Prelude.Try(type);
                return Prelude.Try<Type>(new ContractToTypeException(contract));
            };


        private static Option<Type> TryGetElementType(Type arrayLikeType)
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
            return Prelude.None;
        }
    }
}