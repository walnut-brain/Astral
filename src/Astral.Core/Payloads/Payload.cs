using System;
using System.Collections.Generic;
using System.Net.Mime;
using Astral.Payloads.Contracts;
using Astral.Payloads.Serialization;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Astral.Payloads
{
    public static class Payload
    {
        public static Try<RawPayload> ToRaw(Type type, object obj, ContentType contentType, TypeToContract toContact,
            SerializeProvider<byte[]> serializeProvider)
        {
            type = obj?.GetType() ?? type;
            return
                toContact(type)
                    .Bind(contract =>
                        serializeProvider(contentType)
                            .HeadOrNone()
                            .ToTry(new UnknownContentTypeException($"Unknown content type {contentType}"))
                            .Bind(p => p(obj))
                            .Map(data => new RawPayload(contract, data.Item1, data.Item2)));
        }

        public static Try<TextPayload> ToText(Type type, object obj, ContentType contentType, TypeToContract toContact,
            SerializeProvider<string> serializeProvider)
        {
            type = obj?.GetType() ?? type;
            return
                toContact(type)
                    .Bind(contract =>
                        serializeProvider(contentType)
                            .HeadOrNone()
                            .ToTry(new UnknownContentTypeException($"Unknown content type {contentType}"))
                            .Bind(p => p(obj))
                            .Map(data => new TextPayload(contract, data.Item1, data.Item2)));
        }

        public static Try<RawPayload> ToRaw<T>(T obj, ContentType contentType, TypeToContract toContract,
            SerializeProvider<byte[]> serializeProvider)
        {
            return ToRaw(typeof(T), obj, contentType, toContract, serializeProvider);
        }

        public static Try<TextPayload> ToText<T>(T obj, ContentType contentType, TypeToContract toContract,
            SerializeProvider<string> serializeProvider)
        {
            return ToText(typeof(T), obj, contentType, toContract, serializeProvider);
        }


        public static Try<object> FromRaw(RawPayload payload, Seq<Type> awaited, ContractToType toType,
            DeserializeProvider<byte[]> deserializeProvider)
        {
            return toType(payload.TypeCode, awaited)
                .Bind(type =>
                    deserializeProvider(Optional(payload.ContentType))
                        .Map(d => d(type, payload.Data))
                        .FirstOrError(payload.ContentType?.ToString()));
        }

        public static Try<object> FromText(TextPayload payload, Seq<Type> awaited, ContractToType toType,
            DeserializeProvider<string> deserializeProvider)
        {
            return toType(payload.TypeCode, awaited)
                .Bind(type =>
                    deserializeProvider(Optional(payload.ContentType))
                        .Map(d => d(type, payload.Data))
                        .FirstOrError(payload.ContentType?.ToString()));
        }

        public static Try<T> FromRaw<T>(RawPayload payload, ContractToType toType,
            DeserializeProvider<byte[]> deserializeProvider)
        {
            return FromRaw(payload, typeof(T).Cons(), toType, deserializeProvider).Bind(p => Try(() => (T) p));
        }

        public static Try<T> FromText<T>(TextPayload payload, ContractToType toType,
            DeserializeProvider<string> deserializeProvider)
        {
            return FromText(payload, typeof(T).Cons(), toType, deserializeProvider).Bind(p => Try(() => (T) p));
        }

        internal static Try<T> FirstOrError<T>(this IEnumerable<Try<T>> enumerable, string contentType)
        {
            var exs = new List<Exception>();
            foreach (var p in enumerable)
            {
                var v = p.Try();
                if (!v.IsFaulted)
                    return Try(v.Unwrap());
                v.IfFail(ex => exs.Add(ex));
            }
            if (exs.Count == 0)
                return Try<T>(new UnknownContentTypeException($"Unknown content type {contentType}"));
            return Try<T>(new AggregateException(exs));
        }
    }
}