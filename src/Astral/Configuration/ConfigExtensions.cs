using System;
using System.Runtime.CompilerServices;
using Astral.Configuration.Settings;

namespace Astral.Configuration
{
    public static class ConfigExtensions
    {
        public static TConfig AfterDeliveryTtl<TConfig>(this TConfig config, TimeSpan timeSpan)
            where TConfig : ConfigBase
        {             
            config.SetOption(CheckPredicate(config, new AfterDeliveryTtl(timeSpan)));
            return config;
        }
        
        public static TConfig CleanSameKeyDelivery<TConfig>(this TConfig config, bool clean)
            where TConfig : ConfigBase
        {             
            config.SetOption(CheckPredicate(config, new CleanSameKeyDelivery(clean)));
            return config;
        }
        
        public static TConfig DeliveryLeaseTime<TConfig>(this TConfig config, TimeSpan lease)
            where TConfig : ConfigBase
        {             
            config.SetOption(CheckPredicate(config, new DeliveryLeaseTime(lease)));
            return config;
        }
        
        public static TConfig DeliveryReserveTime<TConfig>(this TConfig config, TimeSpan reserve)
            where TConfig : ConfigBase
        {             
            config.SetOption(CheckPredicate(config, new DeliveryReserveTime(reserve)));
            return config;
        }

        public static TConfig MemberNameConverter<TConfig>(this TConfig config, Func<string, bool, string> converter)
            where TConfig : ConfigBase
        {
            config.SetOption(CheckPredicate(config, new MemberNameToAstralName(converter)));
            return config;
        }

        public static EndpointConfig EndpointName(this EndpointConfig config, string name)
        {             
            config.SetOption(CheckPredicate(config, new EndpointName(name)));
            return config;
        }
        
        public static TConfig MessageTtl<TConfig>(this TConfig config, TimeSpan ttl)
            where TConfig : ConfigBase
        {             
            config.SetOption(CheckPredicate(config, new MessageTtl(ttl)));
            return config;
        }
        
        public static ServiceConfig<T> ServiceName<T>(this ServiceConfig<T> config, string name)
        {             
            config.SetOption(CheckPredicate(config, new ServiceName(name)));
            return config;
        }
        
        private static T CheckPredicate<T>(ConfigBase config, T value, [CallerMemberName] string memberName = "")
        {
            var predicate = config.GetPredicate<T>();
            if (predicate == null) return value;
            if(predicate.True(value)) return value;
            throw new ArgumentException($"Value {value} of {memberName} don't satisfy predicate {predicate.GetType().Name}");

        }
        
        
    }
}