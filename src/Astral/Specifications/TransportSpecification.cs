using System;
using System.Net.Mime;
using Astral.Configuration;
using Astral.Configuration.Settings;
using Astral.Deliveries;
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
    internal class TransportSpecification : IDeliverySpecification
    {
        private readonly EndpointSpecification _specification;
        private string _system;
        private string _transportTag;
        private string _service;
        private string _endpoint;
        private Encode _typeEncoder;
        private SerializeProvider<byte[]> _serializeProvider;

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
        
        public PayloadEncode<byte[]> PayloadEncode => new PayloadEncode<byte[]>(ContentType,
            _specification.GetService<TypeEncoding>().Encode,
            _specification.GetService<Serialization<byte[]>>().Serialize);

        private PayloadDecode<byte[]> PayloadDecode
            => new PayloadDecode<byte[]>(
                _specification.GetService<TypeEncoding>().Decode,
                _specification.GetService<Serialization<byte[]>>().Deserialize);
        
        public Result<Payload<byte[]>> ToPayload(Type type, object obj)
            => Payload.ToPayload(_specification.LoggerFactory.CreateLogger<Payload>(), type, obj, PayloadEncode);

        public Payload.IFromPayload FromPayload(Payload<byte[]> payload)
            => Payload.FromPayload(_specification.LoggerFactory.CreateLogger<Payload>(),  payload, PayloadDecode);
        

        public Result<Payload<byte[]>> ToPayload<T>(T obj) => ToPayload(obj?.GetType() ?? typeof(T), obj);

        string IDeliverySpecification.System => _specification.SystemName;

        string IDeliverySpecification.TransportTag => Tag;

        string IDeliverySpecification.Service => _specification.ServiceName;

        string IDeliverySpecification.Endpoint => _specification.EndpointName;

        Encode IDeliverySpecification.TypeEncoder => _specification.GetService<TypeEncoding>().Encode;

        SerializeProvider<byte[]> IDeliverySpecification.SerializeProvider =>
            _specification.GetService<Serialization<byte[]>>().Serialize; 
    }
}