using System;
using System.Net.Mime;
using Astral.Configuration.Settings;
using Astral.Exceptions;
using Astral.Payloads;
using Astral.Payloads.DataContracts;
using Astral.Payloads.Serialization;
using Astral.Transport;
using FunEx.Monads;
using Microsoft.Extensions.DependencyInjection;

namespace Astral.Configuration.Configs
{
    internal class TransportConfig
    {
        private readonly EndpointConfig _config;

        public TransportConfig(EndpointConfig config)
        {
            _config = config;
            var selector = _config.TryGet<TransportSelector>().Map(p => p.Value);
            Transport = _config.GetService<TransportProvider>().GetTransport(selector.Map(p => ConfigUtils.NormalizeTag(p.Item1)).IfNone(() => ConfigUtils.NormalizeTag(null))).Unwrap();
            TransportTag = selector.Map(p => ConfigUtils.NormalizeTag(p.Item1)).IfNone(() => ConfigUtils.NormalizeTag(null));
            ContentType = selector.Map(p => p.Item2).OrElse(() => _config.TryGet<SerailizationContentType>().Map(p => p.Value))
                .Unwrap(new InvalidConfigurationException($"For {_config.ServiceType}  {_config.PropertyInfo.Name} not setted content type of transport"));
        }
        
        public ITransport Transport { get; }
        public string TransportTag { get; }
        public ContentType ContentType { get; }
        
        public ToPayloadOptions<byte[]> ToPayloadOptions => new ToPayloadOptions<byte[]>(ContentType,
            _config.GetService<TypeEncoding>().ToContract,
            _config.GetService<Serializer<byte[]>>().Serialize);

        private FromPayloadOptions<byte[]> FromPayloadOptions
            => new FromPayloadOptions<byte[]>(
                _config.GetService<TypeEncoding>().ToType,
                _config.GetService<Serializer<byte[]>>().Deserialize);
        
        public Result<Payload<byte[]>> ToPayload(Type type, object obj)
            => Payload.ToPayload(type, obj, ToPayloadOptions);

        public Payload.IFromPayload FromPayload(Payload<byte[]> payload)
            => Payload.FromPayload(payload, FromPayloadOptions);
        

        public Result<Payload<byte[]>> ToPayload<T>(T obj) => ToPayload(obj?.GetType() ?? typeof(T), obj);
    }
}