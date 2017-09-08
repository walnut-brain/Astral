using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using FunEx;
using FunEx.Monads;
using FunEx.TypeClasses;

namespace Astral.Payloads.DataContracts
{
    public static partial class Contract
    {
        
        public static Func<Func<Type, Option<string>>, Func<Type, Result<string>>> Lift(
            Func<Type, Result<string>> lifting) => _ => lifting;

        public static Func<Func<Type, Result<string>>, Func<Type, Result<string>>> Fallback(
            this Func<Func<Type, Result<string>>, Func<Type, Result<string>>> source,
            Func<Func<Type, Result<string>>, Func<Type, Result<string>>> fallback)
            => p => (t => source(p)(t).OrElse(() => source(p)(t)));

        public static Func<Type, Result<string>> Loopback(Func<Func<Type, Result<string>>, Func<Type, Result<string>>> source)
        {
            Result<string> Make(Type type)
            {
                Result<string> Checked(Type tt)
                {
                    if (tt == type)
                        return new RecursiveResolutionException(tt);
                    return Make(tt);

                }
                    
                return source(Checked)(type);
            }

            return Make;
        }
        
        
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