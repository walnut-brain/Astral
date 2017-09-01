using System;
using Astral.Configuration.Settings;
using Astral.DataContracts;
using Astral.Exceptions;
using Astral.Payloads;
using Astral.Payloads.Contracts;
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

        public static Try<string> ContractName<TContract>(this EndpointConfig config, TContract value)
        {
            var type = value?.GetType() ?? typeof(TContract);
            return config
                .TryGet<ITypeToContract>()
                .Bind(p => p.TryMap(type)).ToTry(new TypeToContractException(type));
        }


        public static UseSerializeMapper SerializeMapperUse(this EndpointConfig config)
        {
            return config.AsTry<UseSerializeMapper>().IfFail(UseSerializeMapper.Allow);
        }

        public static Try<PayloadBase<byte[]>> RawSerialize<TContract>(this EndpointConfig config, TContract contract)
        {
            return config
                .ContractName(contract)
                .Bind(p =>
                {
                    var rawSerializer = config.AsTry<ISerialize<byte[]>>();
                    var textSerializer = config.AsTry<ISerialize<string>>();
                    var mapper = config.AsTry<ISerializedMapper<string, byte[]>>();

                    Try<PayloadBase<byte[]>> SerializeThenMap()
                    {
                        return
                            textSerializer
                                .Map(s => s.Serialize(p, contract))
                                .Bind(s => mapper
                                    .Map(m => m.Map(s)));
                    }

                    Try<PayloadBase<byte[]>> SerilizeRaw()
                    {
                        return rawSerializer
                            .Map(s => s.Serialize(p, contract));
                    }

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
                            return Try<PayloadBase<byte[]>>(new ArgumentOutOfRangeException());
                    }
                });
        }

        public static Try<PayloadBase<string>> TextSerialize<TContract>(this EndpointConfig config, TContract contract)
        {
            return config
                .ContractName(contract)
                .Bind(p =>
                    config
                        .AsTry<ISerialize<string>>()
                        .Map(s => s.Serialize(p, contract)));
        }

        public static Try<PayloadBase<byte[]>> RawSerialize<TContract>(this EndpointConfig config, TContract contract,
            PayloadBase<string> textPayload)
        {
            var mapper = config.AsTry<ISerializedMapper<string, byte[]>>();

            Try<PayloadBase<byte[]>> SerilizeRaw()
            {
                return config.AsTry<ISerialize<byte[]>>().Map(s => s.Serialize(textPayload.TypeCode, contract));
            }

            switch (config.SerializeMapperUse())
            {
                case UseSerializeMapper.Never:
                    return SerilizeRaw();
                case UseSerializeMapper.Allow:
                    return mapper
                        .Map(m => m.Map(textPayload))
                        .BindFail(SerilizeRaw);

                case UseSerializeMapper.Always:
                    return mapper
                        .Map(m => m.Map(textPayload));
                default:
                    return Try<PayloadBase<byte[]>>(new ArgumentOutOfRangeException());
            }
        }

        public static Func<Type, PayloadBase<byte[]>, Try<object>> DeserializeRaw(this EndpointConfig config)
        {
            return (type, serialized) =>
            {
                Try<object> RawDeserialize()
                {
                    return config
                        .AsTry<IDeserialize<byte[]>>()
                        .Bind(p => p.Deserialize(type, serialized));
                }

                Try<object> MapThenDeserialize()
                {
                    return config.AsTry<ISerializedMapper<byte[], string>>()
                        .Map(p => p.Map(serialized))
                        .Bind(p => config.AsTry<IDeserialize<string>>()
                            .Bind(s => s.Deserialize(type, p)));
                }

                switch (config.SerializeMapperUse())
                {
                    case UseSerializeMapper.Never:
                        return RawDeserialize();

                    case UseSerializeMapper.Allow:
                        return RawDeserialize().BindFail(MapThenDeserialize);
                    case UseSerializeMapper.Always:
                        return MapThenDeserialize();
                    default:
                        return Try<object>(
                            new ArgumentOutOfRangeException($"Unknown ${nameof(UseSerializeMapper)} value"));
                }
            };
        }
    }
}