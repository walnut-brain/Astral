using System;
using Astral.Payloads.DataContracts;
using Astral.Payloads.Serialization;

namespace Astral.Payloads
{
    public class FromPayloadOptions<TFormat>
    {
        public FromPayloadOptions(ContractToType toType, DeserializeProvider<TFormat> deserializeProvider)
        {
            ToType = toType ?? throw new ArgumentNullException(nameof(toType));
            DeserializeProvider = deserializeProvider ?? throw new ArgumentNullException(nameof(deserializeProvider));
        }

        public ContractToType ToType { get; }
        public DeserializeProvider<TFormat> DeserializeProvider { get; }
    }
}