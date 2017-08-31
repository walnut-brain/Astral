using System;
using LanguageExt;

namespace Astral.Core
{
    public static class DataContract
    {
        public static ITypeToContract ToTypeToContract(this Func<Type, Option<string>> mapper)
            => new TypeToContract(mapper);

        

        public static ITypeToContract ToTypeToContract(this Func<Type, ITypeToContract, Option<string>> source)
        {
            Option<string> Make(Type type)
                => source(type, ToTypeToContract(Make));

            return ToTypeToContract(Make);
        }

        public static ITypeToContract OrElse(this ITypeToContract source, ITypeToContract fallback)
            => ToTypeToContract(type => source.TryMap(type) || fallback.TryMap(type));

        public static ITypeToContract OrElse(this ITypeToContract source, Func<Type, Option<string>> fallback)
            => ToTypeToContract(type => source.TryMap(type) || fallback(type));

        public static Func<Type, ITypeToContract, Option<string>> OrElse(
            this Func<Type, ITypeToContract, Option<string>> source,
            Func<Type, ITypeToContract, Option<string>> next)
            => (type, ttc) => source(type, ttc) || next(type, ttc);

        public static Func<Type, ITypeToContract, Option<string>> OrElse(
            this Func<Type, ITypeToContract, Option<string>> source,
            Func<Type, Option<string>> next)
            => (type, ttc) => source(type, ttc) || next(type);

        public static Func<Type, ITypeToContract, Option<string>> OrElse(
            this Func<Type, Option<string>> source,
            Func<Type, ITypeToContract, Option<string>> next)
            => (type, ttc) => source(type) || next(type, ttc);

        public static Func<Type, ITypeToContract, Option<string>> OrElse(
            this Func<Type, Option<string>> source,
            Func<Type, Option<string>> next)
            => (type, ttc) => source(type) || next(type);

        public static string Map(this ITypeToContract source, Type type)
            => source.TryMap(type).IfNone(() => throw new TypeResolutionException(type));

        public static Option<string> TryMap<T>(this ITypeToContract source, T value)
            => source.TryMap(value?.GetType() ?? typeof(T));

        public static string Map<T>(this ITypeToContract source, T value)
            => source.Map(value?.GetType() ?? typeof(T));


        public static IContractToType ToContractToType(this Func<string, Seq<Type>, Option<Type>> mapper)
            => new ContractToType(mapper);

        public static Type Map(this IContractToType ctt, string contract, Seq<Type> awaited)
            => ctt.TryMap(contract, awaited).IfNone(() => throw new ContractNameResolutionException(contract));

        public static IContractToType ToContractToType(
            this Func<string, Seq<Type>, IContractToType, Option<Type>> source)
        {
            Option<Type> Make(string contract, Seq<Type> awaited)
                => source(contract, awaited, ToContractToType(Make));

            return ToContractToType(Make);
        }

        public static Func<string, Seq<Type>, IContractToType, Option<Type>> OrElse(
            this Func<string, Seq<Type>, IContractToType, Option<Type>> source,
            Func<string, Seq<Type>, IContractToType, Option<Type>> fallback)
            => (contract, awaited, ctt) => source(contract, awaited, ctt) || fallback(contract, awaited, ctt);

        public static Func<string, Seq<Type>, IContractToType, Option<Type>> OrElse(
            this Func<string, Seq<Type>, Option<Type>> source,
            Func<string, Seq<Type>, IContractToType, Option<Type>> fallback)
            => (contract, awaited, ctt) => source(contract, awaited) || fallback(contract, awaited, ctt);

        public static Func<string, Seq<Type>, IContractToType, Option<Type>> OrElse(
            this Func<string, Option<Type>> source,
            Func<string, Seq<Type>, IContractToType, Option<Type>> fallback)
            => (contract, awaited, ctt) => source(contract) || fallback(contract, awaited, ctt);

        public static Func<string, Seq<Type>, IContractToType, Option<Type>> OrElse(
            this Func<string, Seq<Type>, IContractToType, Option<Type>> source,
            Func<string, Seq<Type>, Option<Type>> fallback)
            => (contract, awaited, ctt) => source(contract, awaited, ctt) || fallback(contract, awaited);

        public static Func<string, Seq<Type>, IContractToType, Option<Type>> OrElse(
            this Func<string, Seq<Type>, IContractToType, Option<Type>> source,
            Func<string, Option<Type>> fallback)
            => (contract, awaited, ctt) => source(contract, awaited, ctt) || fallback(contract);

        




        private class TypeToContract : ITypeToContract
        {
            private readonly Func<Type, Option<string>> _mapper;

            public TypeToContract(Func<Type, Option<string>> mapper)
            {
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            }

            public Option<string> TryMap(Type type)
                => _mapper(type);
        }

        private class ContractToType : IContractToType
        {
            private readonly Func<string, Seq<Type>, Option<Type>> _mapper;

            public ContractToType(Func<string, Seq<Type>, Option<Type>> mapper)
            {
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            }

            public Option<Type> TryMap(string contractName, Seq<Type> awaited)
                => _mapper(contractName, awaited);
        } 
    }
}