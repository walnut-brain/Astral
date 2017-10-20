using Astral.Schema;

namespace Astral.Payloads
{
    public interface IRawPayloadSerializer : IPayloadSerializer
    {
        byte[] Serialize<T>(T value, ITypeSchema schema);
        T Deserialize<T>(byte[] body, ITypeSchema schema);
    }
}