using System.Collections.Generic;
using System.Net.Mime;
using Astral.Payloads;
using Astral.Payloads.Json;
using Astral.Schema;
using Newtonsoft.Json;
using RabbitLink.Messaging;

namespace Astral.RabbitLink
{
    internal class DefaultLinkPayloadManager : ILinkPayloadManager
    {
        private readonly PayloadManager _payloadManager;


        public DefaultLinkPayloadManager(JsonSerializerSettings settings = null)
        {
            _payloadManager = new PayloadManager(new JsonPayloadSerializer(settings).AsEnumerable());
        }

        public DefaultLinkPayloadManager(PayloadManager payloadManager)
        {
            _payloadManager = payloadManager;
        }

        public byte[] Serialize<T>(ContentType defaultContentType, T body, LinkMessageProperties props, IReadOnlyCollection<ITypeSchema> knownTypes)
        {
            var payload = _payloadManager.ToRaw(body, knownTypes, defaultContentType.ToString());
            props.ContentType = payload.ContentType;
            props.Type = payload.TypeHint;
            return payload.Body;
        }

        public object Deserialize<T>(ILinkMessage<byte[]> message, IReadOnlyCollection<ITypeSchema> knownTypes)
        {
            var payload = new RawPayload(message.Properties.ContentType, message.Properties.Type, message.Body);
            return _payloadManager.FromRaw<T>(payload, knownTypes);
        }
    }
    
    
}