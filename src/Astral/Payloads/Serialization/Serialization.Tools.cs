using System;
using System.Net.Mime;
using LanguageExt;
using static LanguageExt.Prelude;


namespace Astral.Payloads.Serialization
{
    public static partial class Serialization
    {
        public static Serialize<T> Serialize<T>(Func<object, Try<(ContentType, T)>> func)
        {
            return p => func(p);
        }

        public static Deserialize<T> Deserialize<T>(Func<Type, T, Try<object>> func)
        {
            return (t, d) => func(t, d);
        }

        public static SerializeProvider<T> SerializeProvider<T>(Func<ContentType, bool> support,
            Func<object, Try<T>> serialize)
        {
            return ct => support(ct) ? Serialize(o => serialize(o).Map(r => (ct, r))).Cons() : Seq<Serialize<T>>();
        }

        public static SerializeProvider<T> SerializeProvider<T>(Func<ContentType, bool> support,
            Func<ContentType, object, Try<T>> serialize)
        {
            return ct => support(ct) ? Serialize(o => serialize(ct, o).Map(r => (ct, r))).Cons() : Seq<Serialize<T>>();
        }

        public static DeserializeProvider<T> DeserializeProvider<T>(Func<ContentType, bool> support,
            Deserialize<T> deserialize)
        {
            return ct => ct.Map(support).IfNone(true) ? deserialize.Cons() : Seq<Deserialize<T>>();
        }

        public static DeserializeProvider<T> DeserializeProvider<T>(Func<ContentType, bool> support,
            Func<Option<ContentType>, Deserialize<T>> deserialize)
        {
            return ct => ct.Map(support).IfNone(true) ? deserialize(ct).Cons() : Seq<Deserialize<T>>();
        }

        public static SerializeProvider<T> Combine<T>(this SerializeProvider<T> provider, SerializeProvider<T> other)
        {
            return ct => provider(ct).Append(other(ct));
        }

        public static DeserializeProvider<T> Combine<T>(this DeserializeProvider<T> provider,
            DeserializeProvider<T> other)
        {
            return ct => provider(ct).Append(other(ct));
        }
    }
}