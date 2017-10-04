using System;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Mime;
using Astral.Fun;
using Astral.Fun.Monads;


namespace Astral.Payloads.Serialization
{
    public static partial class Serialization
    {
        public static Serialize<T> Serialize<T>(Func<object, Result<(ContentType, T)>> func)
        {
            return p => func(p);
        }

        public static Deserialize<T> Deserialize<T>(Func<Type, T, Result<object>> func)
        {
            return (t, d) => func(t, d);
        }

        public static SerializeProvider<T> SerializeProvider<T>(Func<ContentType, bool> support,
            Func<object, Result<T>> serialize)
        {
            return ct => support(ct) ? 
                Serialize(o => serialize(o).Map(r => (ct, r))).AsEnumerable() : Enumerable.Empty<Serialize<T>>();
        }

        public static SerializeProvider<T> SerializeProvider<T>(Func<ContentType, bool> support,
            Func<ContentType, object, Result<T>> serialize)
        {
            return ct => support(ct) ? 
                Serialize(o => serialize(ct, o).Map(r => (ct, r))).AsEnumerable() : Enumerable.Empty<Serialize<T>>();
        }

        public static DeserializeProvider<T> DeserializeProvider<T>(Func<ContentType, bool> support,
            Deserialize<T> deserialize)
        {
            return ct => ct.Map(support).IfNone(() => true) ? 
                deserialize.AsEnumerable() : Enumerable.Empty<Deserialize<T>>();
        }

        public static DeserializeProvider<T> DeserializeProvider<T>(Func<ContentType, bool> support,
            Func<Option<ContentType>, Deserialize<T>> deserialize)
        {
            return ct => ct.Map(support).IfNone(() => true) ? 
                deserialize(ct).AsEnumerable() : Enumerable.Empty<Deserialize<T>>();
        }

        public static SerializeProvider<T> Combine<T>(this SerializeProvider<T> provider, SerializeProvider<T> other)
        {
            return ct => provider(ct).Union(other(ct));
        }

        public static DeserializeProvider<T> Combine<T>(this DeserializeProvider<T> provider,
            DeserializeProvider<T> other)
        {
            return ct => provider(ct).Union(other(ct));
        }
    }
}