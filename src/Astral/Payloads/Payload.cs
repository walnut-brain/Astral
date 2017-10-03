using System;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Mime;
using Astral.Payloads.DataContracts;
using FunEx;
using FunEx.Monads;

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

        public static Result<Payload<TFormat>> ToPayload<TFormat>(ITracer logger, Type type, object obj, PayloadEncode<TFormat> payloadEncode)
        {
            type = obj?.GetType() ?? type;
            return
                payloadEncode.ToContact(logger, type)
                    .ToResult(new TypeEncoderException(type))
                    .Bind(contract =>
                        payloadEncode.SerializeProvider(payloadEncode.ContentType)
                            .FirstOrNone()
                            .ToResult(new UnknownContentTypeException($"Unknown content type {payloadEncode.ContentType}"))
                            .Bind(p => p(obj))
                            .Map(data => new Payload<TFormat>(contract, data.Item1, data.Item2)));
        }

        public static Result<Payload<TFormat>> ToPayload<T, TFormat>(ITracer logger, T obj, PayloadEncode<TFormat> payloadEncode) 
            => ToPayload(logger, typeof(T), obj, payloadEncode);


        public static Result<object> FromPayload<TFormat>(ITracer logger, Payload<TFormat> payload, ImmutableList<Type> awaited, PayloadDecode<TFormat> payloadDecode)
        {
            var typ = string.IsNullOrWhiteSpace(payload.TypeCode)
                ? awaited.FirstOrNone()
                : payloadDecode.ToType(logger, payload.TypeCode, awaited);

            return typ
                .ToResult(new TypeDecoderException(payload.TypeCode))
                .Bind(type =>
                {
                    var contentType = payload.ContentType?.ToString();
                    return payloadDecode.DeserializeProvider(
                            payload.ContentType.ToOption())
                        .Select(d => d(type, payload.Data))
                        .FirstOrError(new UnknownContentTypeException($"Unknown content type {contentType}"));
                });
        }


        public interface IFromPayload
        {
            Result<T> As<T>();
        }

        public static IFromPayload FromPayload<TFormat>(ITracer logger, Payload<TFormat> payload, PayloadDecode<TFormat> payloadDecode)
            => new FromPayloadDelegated(p => FromPayload(logger, payload, p, payloadDecode));




        private class FromPayloadDelegated: IFromPayload
        {
            private readonly Func<ImmutableList<Type>, Result<object>> _untyped;

            public FromPayloadDelegated(Func<ImmutableList<Type>, Result<object>> untyped)
            {
                _untyped = untyped;
            }

            public Result<T> As<T>()
                => _untyped(ImmutableList.Create(typeof(T))).Map(p => (T) p);
        }

        
    }
}