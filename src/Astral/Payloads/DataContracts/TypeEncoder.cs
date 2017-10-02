using System;
using System.CodeDom;
using System.Reflection;
using System.Text;
using Astral.Markup;
using FunEx.Monads;
using Microsoft.Extensions.Logging;

namespace Astral.Payloads.DataContracts
{
    public static class TypeEncoder
    {
        public static EncodeComplex Create(string name, Func<Type, Option<string>> encode)
            => (logger, encoder, type) =>
            {
                using (logger.BeginScope("Encoder: {name}", name))
                {
                    logger.LogTrace("type {type}", type);
                    var result = encode(type);
                    logger.LogTrace("result {code}", result);
                    return result;
                }
            };

        public static EncodeComplex Create(string name, Func<Func<Type, Option<string>>, Type, Option<string>> encode)
            => (logger, encoder, type) =>
            {
                Option<string> ControlledEncoder(Type nextType)
                {
                    if (nextType == type)
                        throw new RecursiveResolutionException(type);
                    return encoder(logger, nextType);
                }

                using (logger.BeginScope("Encoder: {name}", name))
                {
                    logger.LogTrace("type {type}", type);

                    var result = encode(ControlledEncoder, type);
                    logger.LogTrace("result {code}", result);
                    return result;
                }
            };

        public static EncodeComplex Fallback(this EncodeComplex a, EncodeComplex b)
            => (logger, encoder, type)
                => a(logger, encoder, type).OrElse(() => b(logger, encoder, type));

        public static Encode Loopback(this EncodeComplex complex)
        {
            Option<string> Loop(ILogger log, Type t) => complex(log, Loop, t);
            return Loop;
        }

        public static bool IsExact<T>(Type type) => typeof(T) == type;

        public static EncodeComplex KnownType<T>(string code)
            => Create(typeof(T).Name, type => IsExact<T>(type) ? (Option<string>) code : Option.None);

        
        public static readonly EncodeComplex ValueTupleEncode = KnownType<ValueTuple>("unit");
        public static readonly EncodeComplex BoolEncode = KnownType<bool>("bool");
        public static readonly EncodeComplex ByteEncode = KnownType<byte>("u8");
        public static readonly EncodeComplex SbyteEncode = KnownType<sbyte>("i8");
        public static readonly EncodeComplex Uint16Encode = KnownType<ushort>("u16");
        public static readonly EncodeComplex Int16Encode = KnownType<short>("i16");
        public static readonly EncodeComplex UintEncode = KnownType<uint>("u32");
        public static readonly EncodeComplex IntEncode = KnownType<int>("i32");
        public static readonly EncodeComplex Uint64Encode = KnownType<ulong>("u64");
        public static readonly EncodeComplex Int64Encode = KnownType<long>("i64");
        public static readonly EncodeComplex FloatEncode = KnownType<float>("f32");
        public static readonly EncodeComplex DoubleEncode = KnownType<double>("f64");
        public static readonly EncodeComplex StringEncode = KnownType<string>("string");
        public static readonly EncodeComplex GuidEncode = KnownType<short>("uuid");
        
        
        public static readonly EncodeComplex WellKnownTypesEncode =
            ValueTupleEncode
                .Fallback(BoolEncode)
                .Fallback(ByteEncode)
                .Fallback(SbyteEncode)
                .Fallback(Uint16Encode)
                .Fallback(Int16Encode)
                .Fallback(UintEncode)
                .Fallback(IntEncode)
                .Fallback(Uint64Encode)
                .Fallback(Int64Encode)
                .Fallback(FloatEncode)
                .Fallback(DoubleEncode)
                .Fallback(StringEncode)
                .Fallback(GuidEncode);
        
        public static readonly EncodeComplex ArrayLikeEncode =
            Create("Array", 
                (resolver, type) => 
                    TypeEncoding
                        .TryGetElementType(type)
                        .Bind(resolver)
                        .Map(p => $"{p}[]"));
        
        public static EncodeComplex AttributedEncoder<T>(Func<T, string> codeExtractor) 
            where T : Attribute
            =>
                Create($"By {typeof(T).Name}", type =>
                {
                    var attr = type.GetCustomAttribute<T>();
                    return attr != null ? (Option<string>) codeExtractor(attr) : Option.None;
                });
        
        public static readonly EncodeComplex Default = WellKnownTypesEncode.Fallback(ArrayLikeEncode)
            .Fallback(AttributedEncoder<ContractAttribute>(a => a.Name));
    }
}