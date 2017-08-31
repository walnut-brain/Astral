using Astral.Core;

namespace Astral.Serialization
{
    public interface ISerializedMapper<TF1, TF2>
    {
        Payload<TF2> Map(Payload<TF1> payload);
    }
}