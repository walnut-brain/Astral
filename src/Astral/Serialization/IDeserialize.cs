using LanguageExt;

namespace WalnutBrain.Bus.Serialization
{
    public interface IDeserialize<TFormat>
    {
        Result<T> Deserialize<T>(Serialized<TFormat> data);
    }
}