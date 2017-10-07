using System;
using System.Collections.Immutable;
using System.Net.Mime;
using Astral.Payloads;
using Astral.Payloads.DataContracts;
using Astral.Payloads.Serialization;
using Astral.Fun.Monads;
using RabbitLink.Messaging;
using RabbitLink.Serialization;

namespace RabbitLink.Services.Astral.Adapters
{
    internal class AstralPayloadManager : IPayloadManager
    {
        private readonly Serialization<byte[]> _serialization;
        private readonly TypeEncoding _typeEncoding;

        public AstralPayloadManager(Serialization<byte[]> serialization, TypeEncoding typeEncoding)
        {
            _serialization = serialization;
            _typeEncoding = typeEncoding;
        }

        public byte[] Serialize<T>(ContentType defaultContentType, T body, LinkMessageProperties props)
        {
            var payload = Payload.ToPayload(new Tracer(), body,
                new PayloadEncode<byte[]>(defaultContentType, _typeEncoding.Encode, _serialization.Serialize)).Unwrap();
            props.ContentType = payload.ContentType.ToString();
            props.Type = payload.TypeCode;
            return payload.Data;
        }

        

        public object Deserialize(ILinkMessage<byte[]> message, Type awaitedType)
        {
            var payload = new Payload<byte[]>(message.Properties.Type, new ContentType(message.Properties.ContentType), message.Body);
            return Payload.FromPayload(new Tracer(), payload, ImmutableList.Create(awaitedType),
                new PayloadDecode<byte[]>(_typeEncoding.Decode, _serialization.Deserialize)).Unwrap();
        }


        private class Tracer : ITracer
        {
            public void Write(string message)
            {
             
            }

            private struct EmptyDisposable : IDisposable
            {
                public void Dispose()
                {
                    
                }
            }
            
            public IDisposable Scope(string name, ushort offset = 4)
                => default(EmptyDisposable);
        }
        
        
    }
    
    
}