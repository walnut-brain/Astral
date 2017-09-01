using Astral.Payloads;

namespace Astral.Serialization
{
    public interface ISerializedMapper<TF1, TF2>
    {
        PayloadBase<TF2> Map(PayloadBase<TF1> payload);
    }
}