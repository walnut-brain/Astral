using System;
using Astral.Configuration.Settings;
using Astral.DataContracts;
using Astral.Exceptions;
using Astral.Serialization;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Astral.Configuration.Configs
{
    public static class ConfigExtensions
    {
        public static Try<T> TryGet<T>(this ConfigBase config)
            => config.GetOption<T>()
                .ToTry(new InvalidConfigurationException($"Cannot find config setting {typeof(T)}"));
        
        public static T Get<T>(this ConfigBase config)
            => config.TryGet<T>().IfFailThrow();

        public static Try<string> ContractName<TContract>(this EndpointConfig config, TContract value)
            => config
                .TryGet<ITypeToContractName>()
                .Bind(p => p.Map(typeof(TContract), value));


        public static UseSerializeMapper SerializeMapperUse(this EndpointConfig config)
            => config.TryGet<UseSerializeMapper>().IfFail(UseSerializeMapper.Allow);

        public static Try<Serialized<byte[]>> RawSerialize<TContract>(this EndpointConfig config, TContract contract)
        {
            return config
                .ContractName(contract)
                .Bind(p =>
                {
                    var rawSerializer = config.TryGet<ISerialize<byte[]>>();
                    var textSerializer = config.TryGet<ISerialize<string>>();
                    var mapper = config.TryGet<ISerializedMapper<string, byte[]>>();

                    Try<Serialized<byte[]>> SerializeThenMap()
                    {
                        return
                            textSerializer
                                .Map(s => s.Serialize(p, contract))
                                .Bind(s => mapper
                                    .Map(m => m.Map(s)));
                    }

                    Try<Serialized<byte[]>> SerilizeRaw() =>
                        rawSerializer
                            .Map(s => s.Serialize(p, contract));

                    switch (config.SerializeMapperUse())
                    {
                        case UseSerializeMapper.Never:
                            return SerilizeRaw();
                        case UseSerializeMapper.Allow:
                            return
                                SerilizeRaw()
                                    .BindFail(SerializeThenMap);
                        case UseSerializeMapper.Always:
                            return SerializeThenMap();
                        default:
                            return Try<Serialized<byte[]>>(new ArgumentOutOfRangeException());
                    }
                });
        }

        public static Try<Serialized<string>> TextSerialize<TContract>(this EndpointConfig config, TContract contract)
        {
            return config
                .ContractName(contract)
                .Bind(p =>
                    config
                        .TryGet<ISerialize<string>>()
                        .Map(s => s.Serialize(p, contract)));

        }

        public static Try<Serialized<byte[]>> RawSerialize<TContract>(this EndpointConfig config, TContract contract,
            Serialized<string> textSerialized)
        {
            
            var mapper = config.TryGet<ISerializedMapper<string, byte[]>>();

            Try<Serialized<byte[]>> SerilizeRaw() =>
                config.TryGet<ISerialize<byte[]>>().Map(s => s.Serialize(textSerialized.TypeCode, contract));

            switch (config.SerializeMapperUse())
            {
                case UseSerializeMapper.Never:
                    return SerilizeRaw();
                case UseSerializeMapper.Allow:
                    return mapper
                        .Map(m => m.Map(textSerialized))
                        .BindFail(SerilizeRaw);

                case UseSerializeMapper.Always:
                    return mapper
                        .Map(m => m.Map(textSerialized));
                default:
                    return Try<Serialized<byte[]>>(new ArgumentOutOfRangeException());
            }
        }

        public static Func<Type, Serialized<byte[]>, Try<object>> DeserializeRaw(this EndpointConfig config)
         => (type, serialized) =>
        {
            
            
            Try<object> RawDeserialize()
                => config
                    .TryGet<IDeserialize<byte[]>>()
                    .Bind( p => p.Deserialize(type, serialized));

            Try<object> MapThenDeserialize()
                =>
                    config.TryGet<ISerializedMapper<byte[], string>>()
                        .Map(p => p.Map(serialized))
                        .Bind(p => config.TryGet<IDeserialize<string>>()
                            .Bind(s => s.Deserialize(type, p)));
            
            switch (config.SerializeMapperUse())
            {
                case UseSerializeMapper.Never:
                    return RawDeserialize();
                        
                case UseSerializeMapper.Allow:
                    return RawDeserialize().BindFail(MapThenDeserialize);
                case UseSerializeMapper.Always:
                    return MapThenDeserialize();
                default:
                    return Try<object>(new ArgumentOutOfRangeException($"Unknown ${nameof(UseSerializeMapper)} value"));
            }
        };

}
}