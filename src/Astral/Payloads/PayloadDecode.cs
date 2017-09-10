using System;
using Astral.Payloads.DataContracts;
using Astral.Payloads.Serialization;

namespace Astral.Payloads
{
    public class PayloadDecode<TFormat>
    {
        public PayloadDecode(Decode toType, DeserializeProvider<TFormat> deserializeProvider)
        {
            ToType = toType ?? throw new ArgumentNullException(nameof(toType));
            DeserializeProvider = deserializeProvider ?? throw new ArgumentNullException(nameof(deserializeProvider));
        }

        public Decode ToType { get; }
        public DeserializeProvider<TFormat> DeserializeProvider { get; }
    }
}