using System;
using System.Collections.Immutable;
using FunEx;

namespace Astral.Payloads.DataContracts
{
    public static partial class Contract
    {
        public static TypeToContract Loopback(this ComplexTypeToContract source)
        {
            Result<string> Make(Type type)
            {
                return source(type, t => t == type ? new RecursiveResolutionException(type).ToFail<string>() : Make(t));
            }

            return Make;
        }

        public static TypeToContract Fallback(this TypeToContract source, TypeToContract fallback)
        {
            return type => source(type).OrElse(() => fallback(type));
        }


        public static ComplexTypeToContract Fallback(
            this ComplexTypeToContract source, ComplexTypeToContract next)
        {
            return (type, ttc) => source(type, ttc).OrElse(() => next(type, ttc));
        }

        public static ComplexTypeToContract Fallback(
            this TypeToContract source, ComplexTypeToContract next)
        {
            return (type, ttc) => source(type).OrElse(() => next(type, ttc));
        }

        public static ComplexTypeToContract Fallback(
            this ComplexTypeToContract source, TypeToContract next)
        {
            return (type, ttc) => source(type, ttc).OrElse(() => next(type));
        }

        public static Result<string> TryMap<T>(this TypeToContract source, T value)
        {
            return source(value?.GetType() ?? typeof(T));
        }


        public static ContractToType Loopback(this ComplexContractToType complex)
        {
            Result<Type> Make(string contract, ImmutableList<Type> awaited)
            {
                return complex(contract, awaited,
                    (c, a) => c == contract && a == awaited
                        ? new RecursiveResolutionException(contract).ToFail<Type>()
                        : Make(c, a));
            }

            return Make;
        }


        public static ComplexContractToType Fallback(
            this ComplexContractToType source,
            ComplexContractToType fallback)
        {
            return (contract, awaited, ctt) =>
                source(contract, awaited, ctt).OrElse(() => fallback(contract, awaited, ctt));
        }

        public static ComplexContractToType Fallback(
            this ComplexContractToType source,
            ContractToType fallback)
        {
            return (contract, awaited, ctt) =>
                source(contract, awaited, ctt).OrElse(() => fallback(contract, awaited));
        }

        public static ComplexContractToType Fallback(
            this ContractToType source,
            ComplexContractToType fallback)
        {
            return (contract, awaited, ctt) =>
                source(contract, awaited).OrElse(() => fallback(contract, awaited, ctt));
        }


        public static ContractToType Fallback(
            this ContractToType source,
            ContractToType fallback)
        {
            return (contract, awaited) => source(contract, awaited).OrElse(() => fallback(contract, awaited));
        }
    }
}