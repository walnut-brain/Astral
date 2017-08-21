using LanguageExt;

namespace Astral.Serialization
{
    public interface IDeserialize<TFormat>
    {
        Result<T> Deserialize<T>(Serialized<TFormat> data);
    }
}