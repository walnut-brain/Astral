using Astral.Core;

namespace Astral.Serialization
{
    public interface ISerialize<TFormat>
    {
        Payload<TFormat> Serialize(string typeCode, object obj);
    }
}