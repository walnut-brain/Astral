namespace Astral.Configuration.Settings
{
    public interface IMessageKeyExtractor<in T>
    {
        string ExtractKey(T message);
    }
}