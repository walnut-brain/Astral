using System;
using System.Net.Mime;
using Astral.Disposables;
using Astral.Payloads;
using Astral.Payloads.DataContracts;
using Astral.Payloads.Serialization;
using FunEx.Monads;
using Microsoft.Extensions.Logging;
using RabbitLink.Messaging;
using RabbitLink.Serialization;

namespace RabbitLink.Services.Astral.Adapters
{
    public class AstralPayloadManager : IPayloadManager
    {
        private readonly Serialization<byte[]> _serialization;
        private readonly TypeEncoding _typeEncoding;

        public AstralPayloadManager(Serialization<byte[]> serialization, TypeEncoding typeEncoding)
        {
            _serialization = serialization;
            _typeEncoding = typeEncoding;
        }

        public ILinkSerializer GetSerializer(ContentType defaultContentType)
            => new LinkSerializer(_serialization, _typeEncoding, defaultContentType,  new Tracer());


        private class Tracer : ITracer
        {
            public void Write(string message)
            {
             
            }

            public IDisposable Scope(string name, ushort offset = 4)
                => Disposable.Empty;
        }
        
        private class LinkSerializer : ILinkSerializer
        {
            private readonly Serialization<byte[]> _serialization;
            private readonly TypeEncoding _typeEncoding;
            private readonly ContentType _contentType;
            private readonly ITracer _tracer;

            public LinkSerializer(Serialization<byte[]> serialization, TypeEncoding typeEncoding, ContentType contentType, ITracer tracer)
            {
                _serialization = serialization;
                _typeEncoding = typeEncoding;
                _contentType = contentType;
                _tracer = tracer;
            }

            public byte[] Serialize<TBody>(TBody body, LinkMessageProperties properties) where TBody : class
            {
                var payload = Payload.ToPayload(_tracer, body,
                    new PayloadEncode<byte[]>(_contentType, _typeEncoding.Encode, _serialization.Serialize)).Unwrap();
                properties.ContentType = payload.ContentType.ToString();
                properties.Type = payload.TypeCode;
                return payload.Data;
            }

            public TBody Deserialize<TBody>(byte[] body, LinkMessageProperties properties) where TBody : class
            {
                var payload = new Payload<byte[]>(properties.Type, new ContentType(properties.ContentType), body);
                return Payload.FromPayload(_tracer, payload,
                    new PayloadDecode<byte[]>(_typeEncoding.Decode, _serialization.Deserialize)).As<TBody>().Unwrap();
            }
        }
    }
    
    
}