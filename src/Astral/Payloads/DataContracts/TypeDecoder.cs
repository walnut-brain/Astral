using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Astral.Fun;
using Astral.Fun.Monads;
using Astral.Markup;

namespace Astral.Payloads.DataContracts
{
    public static class TypeDecoder
    {
        public static DecodeComplex Create(string name, Func<string, Option<Type>> decode)
            => (tracer, decoder, code, awaited) =>
            {
                using (tracer.Scope($"{name}: ", 0))
                {
                    tracer.Write($"code {code}");
                    var result = decode(code);
                    tracer.Write($"result {result}");
                    return result;
                }
            };

        public static DecodeComplex Create(string name, Func<string, IReadOnlyCollection<Type>, Option<Type>> decode)
            => (tracer, decoder, code, awaited) =>
            {
                using (tracer.Scope($"{name}: ", 0))
                {
                    tracer.Write($"code {code}");
                    var result = decode(code, awaited);
                    tracer.Write($"result {result}");
                    return result;
                }
            };

        private static bool CollectionEquals<T>(IReadOnlyCollection<T> c1, IReadOnlyCollection<T> c2)
        {
            if (c1.Count != c2.Count) return false;
            return c1.Zip(c2, (a, b) => Equals(a, b)).All(p => p);
        }

        public static DecodeComplex Create(string name,
            Func<string, IReadOnlyCollection<Type>, Func<string, IReadOnlyCollection<Type>, Option<Type>>, Option<Type>>
                decode)
            => (tracer, decoder, code, awaited) =>
            {
                Option<Type> ControlledDecoder(string nextCode, IReadOnlyCollection<Type> nextAwaited)
                {
                    if (code == nextCode && CollectionEquals(nextAwaited, awaited))
                        throw new RecursiveResolutionException(code);
                    using(tracer.Scope(""))
                        return decoder(tracer, nextCode, nextAwaited);
                }

                using (tracer.Scope($"{name}: ", 0))
                {
                    tracer.Write($"code {code}");
                    var result = decode(code, awaited, ControlledDecoder);
                    tracer.Write($"result {result}");
                    return result;
                }
            };

        public static DecodeComplex Fallback(this DecodeComplex a, DecodeComplex b)
            => (logger, decoder, code, awaited)
                => a(logger, decoder, code, awaited).OrElse(() => b(logger, decoder, code, awaited));

        public static Decode Loopback(this DecodeComplex complex)
        {
            Option<Type> Loop(ITracer tracer, string code, IReadOnlyCollection<Type> awaited) =>
                complex(tracer, Loop, code, awaited);

            return Loop;
        }

        public static DecodeComplex KnownType<T>(string code)
            => Create(typeof(T).Name, c => c == code ? (Option<Type>) typeof(T) : Option.None);

        public static readonly DecodeComplex ValueTupleDecode = KnownType<ValueTuple>("unit");
        public static readonly DecodeComplex BoolDecode = KnownType<bool>("bool");
        public static readonly DecodeComplex ByteDecode = KnownType<byte>("u8");
        public static readonly DecodeComplex SbyteDecode = KnownType<sbyte>("i8");
        public static readonly DecodeComplex Uint16Decode = KnownType<ushort>("u16");
        public static readonly DecodeComplex Int16Decode = KnownType<short>("i16");
        public static readonly DecodeComplex UintDecode = KnownType<uint>("u32");
        public static readonly DecodeComplex IntDecode = KnownType<int>("i32");
        public static readonly DecodeComplex Uint64Decode = KnownType<ulong>("u64");
        public static readonly DecodeComplex Int64Decode = KnownType<long>("i64");
        public static readonly DecodeComplex FloatDecode = KnownType<float>("f32");
        public static readonly DecodeComplex DoubleDecode = KnownType<double>("f64");
        public static readonly DecodeComplex StringDecode = KnownType<string>("string");
        public static readonly DecodeComplex GuidDecode = KnownType<short>("uuid");

        public static readonly DecodeComplex WellKnownTypesDecode =
            ValueTupleDecode
                .Fallback(BoolDecode)
                .Fallback(ByteDecode)
                .Fallback(SbyteDecode)
                .Fallback(Uint16Decode)
                .Fallback(Int16Decode)
                .Fallback(UintDecode)
                .Fallback(IntDecode)
                .Fallback(Uint64Decode)
                .Fallback(Int64Decode)
                .Fallback(FloatDecode)
                .Fallback(DoubleDecode)
                .Fallback(StringDecode)
                .Fallback(GuidDecode);

        public static readonly DecodeComplex ArrayDecode =
            Create("Array", (contract, awaited, resolver) =>
            {
                if (!contract.EndsWith("[]")) return Option.None;
                var elementName = contract.Remove(contract.Length - 2);

                var elementTypes =
                    awaited
                        .SelectMany(p =>
                            TypeEncoding.TryGetElementType(p).Match(t => t.AsEnumerable(), Enumerable.Empty<Type>))
                        .ToImmutableArray();

                return resolver(elementName, elementTypes).Map(p => p.MakeArrayType());
            });

        public static DecodeComplex AttributedDecoder<T>(Func<T, string> codeExtractor)
            where T : Attribute
            =>
                Create($"By {typeof(T).Name}",
                    (contract, awaited, resolver) =>
                    {
                        return
                            awaited
                                .SelectMany(
                                    at =>
                                    {
                                        var attr = at.GetCustomAttribute<T>();
                                        if (attr != null && codeExtractor(attr) == contract)
                                            return at.AsEnumerable();
                                        var attrs = at.GetCustomAttributes<KnownTypeAttribute>();
                                        var nt =
                                            attrs.SelectMany(known =>
                                            {
                                                if (known.MethodName == null) return new[] {known.Type};
                                                var method = at.GetMethod(known.MethodName);
                                                return (IEnumerable<Type>) method.Invoke(null, new object[0]);
                                            }).ToImmutableList();
                                        return resolver(contract, nt).Map(ImmutableList.Create)
                                            .IfNone(ImmutableList<Type>.Empty);
                                    }).FirstOrNone();

                    });

        public static readonly DecodeComplex Default = WellKnownTypesDecode
            .Fallback(ArrayDecode)
            .Fallback(AttributedDecoder<ContractAttribute>(a => a.Name));

    }
}