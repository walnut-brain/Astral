using System;
using System.Collections.Generic;
using System.Net.Mime;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Astral.Payloads
{
    public class Payload<TFormat>
    {
        public Payload(string typeCode, ContentType contentType, TFormat data)
        {
            TypeCode = typeCode;
            ContentType = contentType;
            Data = data;
        }

        public string TypeCode { get; }
        public ContentType ContentType { get; }
        public TFormat Data { get; }
    }


    public static class Payload
    {


        public static Try<Payload<TFormat>> ToPayload<TFormat>(Type type, object obj, ToPayloadOptions<TFormat> toPayloadOptions)
        {
            type = obj?.GetType() ?? type;
            return
                toPayloadOptions.ToContact(type)
                    .Bind(contract =>
                        toPayloadOptions.SerializeProvider(toPayloadOptions.ContentType)
                            .HeadOrNone()
                            .ToTry(new UnknownContentTypeException($"Unknown content type {toPayloadOptions.ContentType}"))
                            .Bind(p => p(obj))
                            .Map(data => new Payload<TFormat>(contract, data.Item1, data.Item2)));
        }

        public static Try<Payload<TFormat>> ToPayload<T, TFormat>(T obj, ToPayloadOptions<TFormat> toPayloadOptions) 
            => ToPayload(typeof(T), obj, toPayloadOptions);


        public static Try<object> FromPayload<TFormat>(Payload<TFormat> payload, Seq<Type> awaited, FromPayloadOptions<TFormat> fromPayloadOptions) 
            => fromPayloadOptions.ToType(payload.TypeCode, awaited)
                .Bind(type =>
                    fromPayloadOptions.DeserializeProvider(
                        Optional(payload.ContentType))
                            .Map(d => d(type, payload.Data))
                            .FirstOrError(payload.ContentType?.ToString()));


        public interface IFromPayload
        {
            Try<T> As<T>();
        }

        public static IFromPayload FromPayload<TFormat>(Payload<TFormat> payload, FromPayloadOptions<TFormat> fromPayloadOptions)
            => new FromPayloadDelegated<TFormat>(p => FromPayload(payload, p, fromPayloadOptions));




        private class FromPayloadDelegated<TFormat> : IFromPayload
        {
            private readonly Func<Seq<Type>, Try<object>> _untyped;

            public FromPayloadDelegated(Func<Seq<Type>, Try<object>> untyped)
            {
                _untyped = untyped;
            }

            public Try<T> As<T>()
                => _untyped(typeof(T).Cons()).Bind(p => Try(() => (T) p));
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