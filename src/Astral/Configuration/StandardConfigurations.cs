using System.Reflection;
using Astral.Configuration.Settings;
using Astral.Core;
using Astral.DataContracts;
using Astral.Lavium;
using Astral.Serialization;
using Astral.Serialization.Json;
using Newtonsoft.Json;
using Prelude = Astral.Lavium.Prelude;

namespace Astral.Configuration
{
    public static class StandardConfigurations
    {

        public static BusConfig UseStandard(this BusConfig config)
        {
            return
                config
                    .UseStdTypeMapper()
                    .UseJson()
                    .UseStdAttributes();
        }

        public static TConfig UseStdTypeMapper<TConfig>(this TConfig config, bool convertNames = false)
            where TConfig : ConfigBase
        {
            var mapper = new DefaultTypeMapper();
            config.SetOption<ITypeToContractName>(mapper);
            config.SetOption<IContractNameToType>(mapper);
            if (convertNames)
                config.SetLaw(Prelude.Law("TypeMapper with type name converter",
                   (MemberNameToAstralName cvt) =>
                   {
                       var map = new DefaultTypeMapper(p => cvt.Value(p, false));
                       return ((ITypeToContractName) map, (IContractNameToType) map);
                   }));
            
            
            return config;
        }

        public static TConfig UseJson<TConfig>(this TConfig config, JsonSerializerSettings jsettings = null, bool checkContentType = false)
            where TConfig : ConfigBase
        {
            jsettings = jsettings ?? new JsonSerializerSettings();
            config.SetOption(jsettings);
            config.SetLaw(Prelude.Law("JsonSerializer", (JsonSerializerSettings settings) =>
                (
                (ISerialize<string>) new JsonTextSerialize(settings),
                (IDeserialize<string>) new JsonTextDeserialize(settings, checkContentType),
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
                    Prelude.Law("ServiceNameFromServiceTypeName", (ServiceType st, MemberNameToAstralName cvt) =>
                        cvt.Value(st.Value.Name, true)),
                    Prelude.Law("ServiceNameFromAttribute", (ServiceType mt) =>
                    {
                        var attr = mt.Value.GetTypeInfo().GetCustomAttribute<ServiceAttribute>();
                        var name = attr?.Name;
                        var version = attr?.Version;
                        return (name == null ? null : new ServiceName(name), 
                            version == null ? null : new ServiceVersion(version));
                    }),
                    Prelude.Law("EndpointNameFromMemberName", (PropertyInfo pi, MemberNameToAstralName cvt) =>
                        cvt.Value(pi.Name, false)),
                    Prelude.Law("EndpointNameFromAttribute", (PropertyInfo mt) =>
                    {
                        var name = mt.GetCustomAttribute<EndpointAttribute>()?.Name;
                        return name == null ? null : new EndpointName(name);
                    })
                });
            return config;
        }
    }
}