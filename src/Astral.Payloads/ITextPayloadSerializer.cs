using Astral.Schema;

namespace Astral.Payloads
{
    public interface ITextPayloadSerializer : IPayloadSerializer
    {
        string Serialize<T>(T value, ITypeSchema schema);
        T Deserialize<T>(string body, ITypeSchema schema);
    }
}