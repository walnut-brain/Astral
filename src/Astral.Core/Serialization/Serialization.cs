using System;
using System.Net.Mime;
using LanguageExt;

namespace Astral.Core
{
    public static class Serialization
    {
        public static ISerialize<TFormat> AsInterface<TFormat>(this Serialize<TFormat> serialize)
            => new DelegateSerialize<TFormat>(serialize);

        public static IDeserialize<TFormat> AsInterface<TFormat>(this Deserialize<TFormat> deserialize)
            => new DelegateDeserialize<TFormat>(deserialize);

    
        private class DelegateDeserialize<TFormat> : IDeserialize<TFormat>
        {
            private readonly Deserialize<TFormat> _deserialize;

            public DelegateDeserialize(Deserialize<TFormat> deserialize)
            {
                _deserialize = deserialize ?? throw new ArgumentNullException(nameof(deserialize));
            }

            public Try<object> Deserialize(Type toType, ContentType contentType, TFormat data)
                => _deserialize(toType, contentType, data);
        }

        private class DelegateSerialize<TFormat> : ISerialize<TFormat>
        {
            private readonly Serialize<TFormat> _serialize;

            public DelegateSerialize(Serialize<TFormat> serialize)
            {
                _serialize = serialize ?? throw new ArgumentNullException(nameof(serialize));
            }

            public Try<(ContentType, TFormat)> Serialize(Type toType, object obj)
                => _serialize(toType, obj);
        }
    }
}