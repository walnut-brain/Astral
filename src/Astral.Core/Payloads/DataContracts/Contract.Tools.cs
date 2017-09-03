using System;
using LanguageExt;

namespace Astral.Payloads.DataContracts
{
    public static partial class Contract
    {
        public static TypeToContract Loopback(this ComplexTypeToContract source)
        {
            Try<string> Make(Type type)
            {
                return source(type, t => t == type ? Prelude.Try<string>(new RecursiveResolutionException(type)) : Make(t));
            }

            return Make;
        }

        public static TypeToContract Fallback(this TypeToContract source, TypeToContract fallback)
        {
            return type => source(type).BiBind(Prelude.Try, _ => fallback(type));
        }


        public static ComplexTypeToContract Fallback(
            this ComplexTypeToContract source, ComplexTypeToContract next)
        {
            return (type, ttc) => source(type, ttc).BiBind(Prelude.Try, _ => next(type, ttc));
        }

        public static ComplexTypeToContract Fallback(
            this TypeToContract source, ComplexTypeToContract next)
        {
            return (type, ttc) => source(type).BiBind(Prelude.Try, _ => next(type, ttc));
        }

        public static ComplexTypeToContract Fallback(
            this ComplexTypeToContract source, TypeToContract next)
        {
            return (type, ttc) => source(type, ttc).BiBind(Prelude.Try, _ => next(type));
        }

        public static Try<string> TryMap<T>(this TypeToContract source, T value)
        {
            return source(value?.GetType() ?? typeof(T));
        }


        public static ContractToType Loopback(this ComplexContractToType complex)
        {
            Try<Type> Make(string contract, Seq<Type> awaited)
            {
                return complex(contract, awaited,
                    (c, a) => c == contract && a == awaited
                        ? Prelude.Try<Type>(new RecursiveResolutionException(contract))
                        : Make(c, a));
            }

            return Make;
        }


        public static ComplexContractToType Fallback(
            this ComplexContractToType source,
            ComplexContractToType fallback)
        {
            return (contract, awaited, ctt) =>
                source(contract, awaited, ctt).BiBind(Prelude.Try, _ => fallback(contract, awaited, ctt));
        }

        public static ComplexContractToType Fallback(
            this ComplexContractToType source,
            ContractToType fallback)
        {
            return (contract, awaited, ctt) =>
                source(contract, awaited, ctt).BiBind(Prelude.Try, _ => fallback(contract, awaited));
        }

        public static ComplexContractToType Fallback(
            this ContractToType source,
            ComplexContractToType fallback)
        {
            return (contract, awaited, ctt) =>
                source(contract, awaited).BiBind(Prelude.Try, _ => fallback(contract, awaited, ctt));
        }


        public static ContractToType Fallback(
            this ContractToType source,
            ContractToType fallback)
        {
            return (contract, awaited) => source(contract, awaited).BiBind(Prelude.Try, _ => fallback(contract, awaited));
        }
    }
}