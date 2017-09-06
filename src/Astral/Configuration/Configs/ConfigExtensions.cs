using Astral.Exceptions;
using FunEx;

namespace Astral.Configuration.Configs
{
    public static class ConfigExtensions
    {
        public static Result<T> AsTry<T>(this ConfigBase config)
        {
            return config.TryGet<T>()
                .ToResult(new InvalidConfigurationException($"Cannot find config setting {typeof(T)}"));
        }

        public static bool TryGet<T>(this ConfigBase config, out T result)
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

        public static T Get<T>(this ConfigBase config)
        {
            return config.AsTry<T>().Unwrap();
        }

        
    }
}