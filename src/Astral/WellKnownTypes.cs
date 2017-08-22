using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Astral
{
    public static class WellKnownTypes
    {

        private static readonly IReadOnlyDictionary<Type, (string code, string cstype)> Types =
            new ReadOnlyDictionary<Type, (string, string)>(new Dictionary<Type, (string, string)>()
            {
                {typeof(bool) , ("bool", "bool") },
                {typeof(byte), ("u8", "byte") },
                {typeof(sbyte) , ("i8", "sbyte)") },
                {typeof(ushort),  ("u16", "ushort") },
                {typeof(short), ("i16", "short") },
                {typeof(int), ("i32", "int")  },
                {typeof(uint), ("u32", "uint") },
                {typeof(long), ("i64", "long")  },
                {typeof(ulong), ("u64", "ulong") },
                {typeof(double), ("double", "double") },
                {typeof(float), ("float", "float")  },
                {typeof(string), ("string", "string") },
                {typeof(byte[]), ("bytes", "byte[]")  },
                {typeof(JToken), ("json", typeof(JToken).FullName) },
                {typeof(JObject),  ("jobject", typeof(JObject).FullName) },
                {typeof(JArray), ("jarray", typeof(JArray).FullName) },
                {typeof(JValue), ("jvalue", typeof(JValue).FullName) },
            });


        public static readonly IReadOnlyDictionary<string, Type> TypeByCode =
            new ReadOnlyDictionary<string, Type>(Types.ToDictionary(p => p.Value.code, p => p.Key));

        public static readonly IReadOnlyDictionary<Type, string> CodeByType =
            new ReadOnlyDictionary<Type, string>(TypeByCode.ToDictionary(p => p.Value, p => p.Key));

        public static string UnitTypeCode { get; } = "unit";
        public static string FailTypeCode { get; } = "error";

        public static Type[] UnitTypes { get; set; } = { typeof(ValueTuple) };

        public static string GetCSharpTypeByCode(string code)
        {
            var type = TypeByCode[code];
            return Types[type].cstype;
        }
    }
}
