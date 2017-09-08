using Astral.Exceptions;
using FunEx;
using FunEx.Monads;

namespace Astral.Configuration.Configs
{
    public static class ConfigExtensions
    {
        public static Result<T> AsTry<T>(this ConfigBase config) where T : Fact
        {
            return config.TryGet<T>()
                .ToResult(new InvalidConfigurationException($"Cannot find config setting {typeof(T)}"));
        }

        public static bool TryGet<T>(this ConfigBase config, out T result) where T : Fact
        {
            var data = default(T);
            var res = config.TryGet<T>().Match(p =>
            {
                data = p;
                return true;
            }, () => false);
            result = data;
            return res;
        }

        public static T Get<T>(this ConfigBase config) where T : Fact
        {
            return config.AsTry<T>().Unwrap();
        }

        
    }
}