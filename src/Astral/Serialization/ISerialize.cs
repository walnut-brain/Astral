using Astral.Payloads;

namespace Astral.Serialization
{
    public interface ISerialize<TFormat>
    {
        PayloadBase<TFormat> Serialize(string typeCode, object obj);
    }
}