namespace Astral.Schema
{
    public interface ISchema
    {
        bool TryGetProperty<T>(string property, out T value);
    }
}