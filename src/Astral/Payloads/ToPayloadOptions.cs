using System;
using System.Net.Mime;
using Astral.Payloads.DataContracts;
using Astral.Payloads.Serialization;

namespace Astral.Payloads
{
    public class ToPayloadOptions<TFormat>
    {
        public ToPayloadOptions(ContentType contentType, TypeToContract toContact, SerializeProvider<TFormat> serializeProvider)
        {
            ContentType = contentType ?? throw new ArgumentNullException(nameof(contentType));
            ToContact = toContact ?? throw new ArgumentNullException(nameof(toContact));
            SerializeProvider = serializeProvider ?? throw new ArgumentNullException(nameof(serializeProvider));
        }

        public ContentType ContentType { get; }
        public TypeToContract ToContact { get; }
        public SerializeProvider<TFormat> SerializeProvider { get; }
    }
}