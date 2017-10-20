using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using Astral.Schema;

namespace Astral.Payloads
{
    public class PayloadManager
    {
        public IEnumerable<IPayloadSerializer> Serializers { get; }

        public PayloadManager(IEnumerable<IPayloadSerializer> serializers)
        {
            Serializers = serializers is ICollection<IPayloadSerializer> c ? c : serializers.ToList();
        }

        public ITextPayloadSerializer GetTextSerializer(string contentType)
        {
            var serializer = Serializers.OfType<ITextPayloadSerializer>().FirstOrDefault(p => string.IsNullOrWhiteSpace(contentType) || p.SupportContentType(contentType));
            if(serializer == null)
                throw new ArgumentOutOfRangeException($"No text serializers for content type {contentType} registered");
            return serializer;
        }
        
        public IRawPayloadSerializer GetRawSerializer(string contentType)
        {
            var serializer = Serializers.OfType<IRawPayloadSerializer>().FirstOrDefault(p => string.IsNullOrWhiteSpace(contentType) || p.SupportContentType(contentType));
            if (serializer == null)
            {
                var charset = "utf-8";
                var ctText = (string) null;
                if (!string.IsNullOrWhiteSpace(contentType))
                {
                    var ct = new ContentType(contentType);
                    if (!string.IsNullOrWhiteSpace(ct.CharSet))
                    {
                        charset = ct.CharSet;
                        ct.CharSet = null;
                    }
                    ctText = ct.ToString();
                }


                ITextPayloadSerializer textSerializer;
                try
                {
                    textSerializer = GetTextSerializer(ctText);
                }
                catch (ArgumentOutOfRangeException aoorEx)
                {
                    throw new ArgumentOutOfRangeException($"No raw or text serializers for content type {contentType} registered");
                }
                
                return new TextRawSerializer(textSerializer, Encoding.GetEncoding(charset));
            }
            return serializer;
        }
        
         
        
        
        public TextPayload ToText<T>(T value, IReadOnlyCollection<ITypeSchema> knownTypes, string contentType)
        {
            if (knownTypes == null) throw new ArgumentNullException(nameof(knownTypes));
            var serializer = GetTextSerializer(contentType);
            var type = knownTypes.FirstOrDefault(p => p.DotNetType == (value?.GetType() ?? typeof(T)));
            var typeHint = type?.ContractName ?? type?.SchemaName;
            return new TextPayload(contentType, typeHint, serializer.Serialize(value, type));
        }
        
        public RawPayload ToRaw<T>(T value, IReadOnlyCollection<ITypeSchema> knownTypes, string contentType)
        {
            if (knownTypes == null) throw new ArgumentNullException(nameof(knownTypes));
            var serializer = GetRawSerializer(contentType);
            var type = knownTypes.FirstOrDefault(p => p.DotNetType == (value?.GetType() ?? typeof(T)));
            var typeHint = type?.ContractName ?? type?.SchemaName;
            return new RawPayload(contentType, typeHint, serializer.Serialize(value, type));
        }

        public object FromText<T>(TextPayload payload, IReadOnlyCollection<ITypeSchema> knownTypes)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));
            var serializer = GetTextSerializer(payload.ContentType);
            var typeSchema = knownTypes.FirstOrDefault(p => p.ContractName == payload.TypeHint)
                       ?? knownTypes.FirstOrDefault(p => p.SchemaName == payload.TypeHint);
            var type = typeSchema?.DotNetType ?? typeof(T);
            if (type == typeof(T))
                return serializer.Deserialize<T>(payload.Body, typeSchema);
            var gMethod = typeof(ITextPayloadSerializer).GetMethod(nameof(ITextPayloadSerializer.Deserialize));
            var method = gMethod.MakeGenericMethod(type);
            return method.Invoke(serializer, new object[] {payload.Body, typeSchema});
        }
        
        public object FromRaw<T>(RawPayload payload, IReadOnlyCollection<ITypeSchema> knownTypes)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));
            var serializer = GetRawSerializer(payload.ContentType);

            var typeSchema = payload.TypeHint != null
                ? knownTypes.FirstOrDefault(p => p.ContractName == payload.TypeHint)
                  ?? knownTypes.FirstOrDefault(p => p.SchemaName == payload.TypeHint)
                : null;
            var type = typeSchema?.DotNetType ?? typeof(T);
            if (type == typeof(T))
                return serializer.Deserialize<T>(payload.Body, typeSchema);
            var gMethod = typeof(IRawPayloadSerializer).GetMethod(nameof(IRawPayloadSerializer.Deserialize));
            var method = gMethod.MakeGenericMethod(type);
            return method.Invoke(serializer, new object[] {payload.Body, typeSchema});
        }

        private class TextRawSerializer : IRawPayloadSerializer
        {
            private readonly ITextPayloadSerializer _serializer;
            private readonly Encoding _encoding;

            public TextRawSerializer(ITextPayloadSerializer serializer, Encoding encoding)
            {
                _serializer = serializer;
                _encoding = encoding;
            }

            public bool SupportContentType(string contentType) => throw new NotSupportedException();

            public byte[] Serialize<T>(T value, ITypeSchema schema)
            {
                return _encoding.GetBytes(_serializer.Serialize(value, schema));
            }

            public T Deserialize<T>(byte[] body, ITypeSchema schema)
            {
                return _serializer.Deserialize<T>(_encoding.GetString(body), schema);
            }
        }
    }
}