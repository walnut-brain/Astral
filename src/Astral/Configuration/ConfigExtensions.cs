using Astral.Exceptions;
using FunEx;
using FunEx.Monads;

namespace Astral.Specifications
{
    public static class ConfigExtensions
    {
        public static Result<T> AsTry<T>(this ConfigBase config) where T : Fact
        {
            return config.TryGetService<T>()
                .ToResult(new InvalidConfigurationException($"Cannot find config setting {typeof(T)}"));
        }

        public static bool TryGetService<T>(this ConfigBase config, out T result) where T : Fact
        {
            var data = default(T);
            var res = config.TryGetService<T>().Match(p =>
            {
                data = p;
                return true;
            }, () => false);
            result = data;
            return res;
        }

        

        
    }
}