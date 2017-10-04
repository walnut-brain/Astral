using System;
using System.Collections.Generic;
using System.Linq;
using Astral.Fun.Monads;

namespace Astral.Payloads.DataContracts
{
    public class TypeEncoding
    {
        public TypeEncoding(Encode encode, Decode decode)
        {
            Encode = encode;
            Decode = decode;
        }

        public Encode Encode { get; }
        public Decode Decode { get; }


        public static readonly TypeEncoding Default =
            new TypeEncoding(
                TypeEncoder.Default.Loopback(),
                TypeDecoder.Default.Loopback());

        


        internal static Option<Type> TryGetElementType(Type arrayLikeType)
        {
            if (arrayLikeType.IsArray)
            {
                var elementType = arrayLikeType.GetElementType();
                return elementType.ToOption();
            }
            var enumIntf = arrayLikeType.GetInterfaces()
                .FirstOrDefault(p => p.IsGenericType && p.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            if (enumIntf == null) return Option.None;
            {
                var elementType = enumIntf.GetGenericArguments()[0];
                return elementType.ToOption();
            }
        }
    }
}