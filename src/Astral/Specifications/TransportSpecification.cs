using System;
using System.Net.Mime;
using Astral.Configuration;
using Astral.Configuration.Settings;
using Astral.Exceptions;
using Astral.Payloads;
using Astral.Payloads.DataContracts;
using Astral.Payloads.Serialization;
using Astral.Transport;
using FunEx;
using FunEx.Monads;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Astral.Specifications
{
    internal class TransportSpecification
    {
        private readonly EndpointSpecification _specification;

        public TransportSpecification(EndpointSpecification specification)
        {
            _specification = specification;
            var selector = _specification.TryGetService<TransportSelector>().Map(p => p.Value);
            Provider = _specification.GetService<TransportProvider>().GetTransport(selector.Map(p => ConfigUtils.NormalizeTag(p.Item1)).IfNone(() => ConfigUtils.NormalizeTag(null))).Unwrap();
            Tag = selector.Map(p => ConfigUtils.NormalizeTag(p.Item1)).IfNone(() => ConfigUtils.NormalizeTag(null));
            ContentType = selector.Map(p => p.Item2).OrElse(() => _specification.TryGetService<SerailizationContentType>().Map(p => p.Value))
                .Unwrap(new InvalidConfigurationException($"For {_specification.ServiceType}  {_specification.PropertyInfo.Name} not setted content type of transport"));
        }
        
        public ITransport Provider { get; }
        public string Tag { get; }
        public ContentType ContentType { get; }
        
        public ToPayloadOptions<byte[]> ToPayloadOptions => new ToPayloadOptions<byte[]>(ContentType,
            _specification.GetService<TypeEncoding>().Encode,
            _specification.GetService<Serialization<byte[]>>().Serialize);

        private FromPayloadOptions<byte[]> FromPayloadOptions
            => new FromPayloadOptions<byte[]>(
                _specification.GetService<TypeEncoding>().Decode,
                _specification.GetService<Serialization<byte[]>>().Deserialize);
        
        public Result<Payload<byte[]>> ToPayload(Type type, object obj)
            => Payload.ToPayload(_specification.LoggerFactory.CreateLogger<Payload>(), type, obj, ToPayloadOptions);

        public Payload.IFromPayload FromPayload(Payload<byte[]> payload)
            => Payload.FromPayload(_specification.LoggerFactory.CreateLogger<Payload>(),  payload, FromPayloadOptions);
        

        public Result<Payload<byte[]>> ToPayload<T>(T obj) => ToPayload(obj?.GetType() ?? typeof(T), obj);
    }
}