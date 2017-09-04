using System;
using Astral.Configuration.Settings;
using Astral.DataContracts;
using Astral.Exceptions;
using Astral.Payloads;
using Astral.Payloads.DataContracts;
using Astral.Serialization;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Astral.Configuration.Configs
{
    public static class ConfigExtensions
    {
        public static Try<T> AsTry<T>(this ConfigBase config)
        {
            return config.TryGet<T>()
                .ToTry(new InvalidConfigurationException($"Cannot find config setting {typeof(T)}"));
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
            return config.AsTry<T>().IfFailThrow();
        }

        
    }
}