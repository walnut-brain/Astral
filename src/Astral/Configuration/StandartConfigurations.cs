using System.Reflection;
using Astral.Configuration.Settings;
using Astral.Lavium;
using Astral.Serialization;
using Astral.Serialization.Json;
using Newtonsoft.Json;
using Prelude = Astral.Lavium.Prelude;

namespace Astral.Configuration
{
    public static class StandartConfigurations
    {
        public static TConfig UseJson<TConfig>(this TConfig config, JsonSerializerSettings jsettings = null)
            where TConfig : ConfigBase
        {
            jsettings = jsettings ?? new JsonSerializerSettings();
            config.SetOption(jsettings);
            config.SetLaw(Prelude.Law("JsonSerializer", (JsonSerializerSettings settings) =>
                (
                (ISerialize<string>) new JsonTextSerialize(settings),
                (IDeserialize<string>) new JsonTextDeserialize(settings),
                (ISerializedMapper<string, byte[]>) new Utf8Mapper(),
                (ISerializedMapper<byte[], string>) new Utf8BackMapper(),
                UseSerializeMapper.Always
                )
            ));
            return config;
        }

        public static TConfig UseStdAttributes<TConfig>(this TConfig config)
            where TConfig : ConfigBase
        {
            config.SetLawSet(
                new Law[]
                {
                    /*
                    Prelude.Law("ContractNameFromAttribute", (MessageType mt) =>
                                           {
                                               var name = mt.Value.GetTypeInfo().GetCustomAttribute<ContractNameAttribute>()?.Name;
                                               return name == null ? null : new ContractName(name);
                                           }),
                    Prelude.Law("ResponseContractNameFromAttribute", (ResponseType mt) =>
                    {
                        var name = mt.Value.GetTypeInfo().GetCustomAttribute<ContractNameAttribute>()?.Name;
                        return name == null ? null : new ResponseContractName(name);
                    }),
                    
                    Prelude.Law("ServiceNameFromAttribute", (ServiceType mt) =>
                    {
                        var name = mt.Value.GetTypeInfo().GetCustomAttribute<ServiceNameAttribute>()?.Name;
                        return name == null ? null : new ServiceName(name);
                    }),
                    Prelude.Law("EndpointNameFromAttribute", (PropertyInfo mt) =>
                    {
                        var name = mt.GetCustomAttribute<EndpointNameAttribute>()?.Name;
                        return name == null ? null : new EndpointName(name);
                    })
                    
                    Prelude.Law("MessageTtlFromAttribute", (PropertyInfo mt) =>
                    {
                        var ttl = mt.GetCustomAttribute<MessageTtlAttribute>()?.Ttl;
                        return ttl == null ? null : new MessageTtl(ttl.Value);
                    } */
                });
            return config;
        }
    }
}