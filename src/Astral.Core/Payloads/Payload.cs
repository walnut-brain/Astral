using System;
using System.Collections.Generic;
using System.Net.Mime;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Astral.Payloads
{
    public class Payload<TFormat> : Payload
    {
        public Payload(string typeCode, ContentType contentType, TFormat data)
            : base(typeCode, contentType)
        {
            Data = data;
        }

        public TFormat Data { get; }

        public override Type Format => typeof(TFormat);
        public override object Value => Data;
    }


    public abstract class Payload 
    {
        internal Payload(string typeCode, ContentType contentType)
        {
            TypeCode = typeCode;
            ContentType = contentType;
        }

        public string TypeCode { get; }
        public ContentType ContentType { get; }

        public abstract Type Format { get; }
        public abstract object Value { get; }

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
                {
                    string contentType = payload.ContentType?.ToString();
                    return fromPayloadOptions.DeserializeProvider(
                            Optional(payload.ContentType))
                        .Map(d => d(type, payload.Data))
                        .FirstOrError(new UnknownContentTypeException($"Unknown content type {contentType}"));
                });


        public interface IFromPayload
        {
            Try<T> As<T>();
        }

        public static IFromPayload FromPayload<TFormat>(Payload<TFormat> payload, FromPayloadOptions<TFormat> fromPayloadOptions)
            => new FromPayloadDelegated(p => FromPayload(payload, p, fromPayloadOptions));




        private class FromPayloadDelegated: IFromPayload
        {
            private readonly Func<Seq<Type>, Try<object>> _untyped;

            public FromPayloadDelegated(Func<Seq<Type>, Try<object>> untyped)
            {
                _untyped = untyped;
            }

            public Try<T> As<T>()
                => _untyped(typeof(T).Cons()).Bind(p => Try(() => (T) p));
        }

        
    }
}