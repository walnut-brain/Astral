using LanguageExt;

namespace Astral.Configuration
{
    public interface IConfigOptions
    {
        Option<T> Get<T>();
    }
}