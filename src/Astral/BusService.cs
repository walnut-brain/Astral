using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Astral;
using Astral.Configuration;
using Astral.Configuration.Configs;
using Astral.Configuration.Settings;
using Astral.Core;
using Astral.Data;
using Astral.DataContracts;
using Astral.DependencyInjection;
using Astral.Exceptions;
using Astral.Serialization;
using Astral.Transport;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Astral
{
    public class BusService<TService> : IEventSource<TService>, IEventSubscriber<TService>
    {
        private readonly ServiceConfig<TService> _serviceConfig;


        internal BusService(ServiceConfig<TService> serviceConfig)
        {
            _serviceConfig = serviceConfig ?? throw new ArgumentNullException(nameof(serviceConfig));
        }

        private IMqTransport GetMqTransport(EndpointConfig config)
        {
            throw new NotImplementedException();
        }

        private readonly ILogger _logger;
        

        public Task PublishAsync<TEvent>(Expression<Func<TService, IEvent<TEvent>>> selector, TEvent @event, EventPublishOptions options = null)
        {
            var endpointConfig = _serviceConfig.Endpoint(selector);
            var transport = GetMqTransport(endpointConfig);
            var typeToContract = endpointConfig.Get<ITypeToContractName>();
            var contractName = typeToContract.Map(typeof(TEvent), @event);
            var useMapper = endpointConfig.TryGet<UseSerializeMapper>().IfNone(() => UseSerializeMapper.Allow);
            var mapperOpt = endpointConfig.TryGet<ISerializedMapper<string, byte[]>>();
            var poptions = new PublishOptions(
                options?.EventTtl ?? endpointConfig.TryGet<MessageTtl>().Map(p => p.Value).IfNone(Timeout.InfiniteTimeSpan));
            
            Serialized <byte[]> serialized;
            switch (useMapper)
            {
                case UseSerializeMapper.Never:
                case UseSerializeMapper.Allow when mapperOpt.IsNone:
                    var rawSerailizer = endpointConfig.Get<ISerialize<byte[]>>();
                    serialized = rawSerailizer.Serialize(contractName, @event);
                    break;
                
                case UseSerializeMapper.Always:
                case UseSerializeMapper.Allow when mapperOpt.IsSome:
                    var mapper = mapperOpt.Unwrap();
                    var textSerializer = endpointConfig.Get<ISerialize<string>>();
                    serialized = mapper.Map(textSerializer.Serialize(contractName, @event));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var prepared = transport.PreparePublish(endpointConfig, @event, serialized, poptions);
            return prepared();
        }

        public Action EnqueueManual<TUoW, TEvent>(IDeliveryDataService<TUoW> dataService, Expression<Func<TService, IEvent<TEvent>>> selector, TEvent @event,
            EventPublishOptions options = null) where TUoW : IUnitOfWork
        {
            var endpointConfig = _serviceConfig.Endpoint(selector);
            var typeToContract = endpointConfig.Get<ITypeToContractName>();
            var contractName = typeToContract.Map(typeof(TEvent), @event);
            var textSerializer = endpointConfig.Get<ISerialize<string>>();
            var serialized = textSerializer.Serialize(contractName, @event);
            var reserveTime = endpointConfig.TryGet<DeliveryReserveTime>().Map(p => p.Value)
                .IfNone(TimeSpan.FromSeconds(3));
            var deliveryId = Guid.NewGuid();
            var key = endpointConfig.TryGet<IMessageKeyExtractor<TEvent>>().Map(p => p.ExtractKey(@event)) ||
                      Prelude.Optional(@event as IKeyProvider).Map(p => p.Key);
            var serviceName = endpointConfig.Get<ServiceName>().Value;
            var endpointName = endpointConfig.Get<EndpointName>().Value;
            key.Filter(_ => endpointConfig.TryGet<CleanSameKeyDelivery>().Map(p => p.Value).IfNone(true))
                .IfSome(k => dataService.DeleteAll(serviceName, endpointName, k));
            var messageTtl = options?.EventTtl ??
                             endpointConfig.TryGet<MessageTtl>().Map(p => p.Value).IfNone(Timeout.InfiniteTimeSpan);



            dataService.Create(new DeliveryRecord(deliveryId, 
                serviceName,
                endpointName,
                serialized.TypeCode,
                serialized.ContentType.ToString(),
                serialized.Data,
                key.IfNoneUnsafe((string) null),
                DateTimeOffset.Now + reserveTime,
                null,
                false,
                false,
                messageTtl < TimeSpan.Zero ? (DateTimeOffset?) null : DateTimeOffset.Now + messageTtl,
                null,
                null,
                0,
                null));

            throw new NotImplementedException();
        }

        private Action Deliver<T, TUoW>(EndpointConfig config, 
            DeliveryRecord record,
            Serialized<string> serialized, 
            T message)
        {
            var useMapper = config.TryGet<UseSerializeMapper>().IfNone(() => UseSerializeMapper.Allow);
            var mapperOpt = config.TryGet<ISerializedMapper<string, byte[]>>();
            using (var scope = _logger.BeginScope("Delivery {service} {endpoint} {isAnswer}", record.ServiceName,
                record.EndpointName,
                record.IsAnswer))
            {
                Serialized<byte[]> rawSerialized;
                try
                {
                    switch (useMapper)
                    {
                        case UseSerializeMapper.Never:
                        case UseSerializeMapper.Allow when mapperOpt.IsNone:
                            var rawSerailizer = config.Get<ISerialize<byte[]>>();
                            rawSerialized = rawSerailizer.Serialize(serialized.TypeCode, message);
                            break;

                        case UseSerializeMapper.Always:
                        case UseSerializeMapper.Allow when mapperOpt.IsSome:
                            var mapper = mapperOpt.Unwrap("Mapper not found");
                            rawSerialized = mapper.Map(serialized);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                catch (Exception ex)
                {

                }
            }

            throw new NotImplementedException();
        }


        
        

        public IDisposable Subscribe<TEvent>(Expression<Func<TService, IEvent<TEvent>>> selector, 
            IEventHandler<TEvent> eventHandler, 
            EventSubscribeOptions options = null)
        {
            var endpointConfig = _serviceConfig.Endpoint(selector);
            var transport = GetMqTransport(endpointConfig);
            var exceptionPolicy = endpointConfig.TryGet<IReciveExceptionPolicy>().IfNone(new DefaultExceptionPolicy());
            
            var resolver = endpointConfig.Get<IContractNameToType>();

            var useMapper = endpointConfig.TryGet<UseSerializeMapper>().IfNone(() => UseSerializeMapper.Allow);
            var mapperOpt = endpointConfig.TryGet<ISerializedMapper<byte[], string>>();
            Func<Type, Serialized<byte[]>, Try<object>> deserialize;
            switch (useMapper)
            {
                case UseSerializeMapper.Never:
                case UseSerializeMapper.Allow when mapperOpt.IsNone:
                    deserialize = endpointConfig.Get<IDeserialize<byte[]>>().Deserialize;
                    break;
                case UseSerializeMapper.Always:
                case UseSerializeMapper.Allow when mapperOpt.IsSome:
                    var mapper = mapperOpt.Unwrap("Serialization mapper not detected");
                    var textDeserialize = endpointConfig.Get<IDeserialize<string>>();

                    deserialize = (t, data) =>
                        Prelude
                            .Try(() => mapper.Map(data))
                            .Bind(p => textDeserialize.Deserialize(t, p));
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unknown ${nameof(UseSerializeMapper)} value {useMapper}");
            }

            var ignoreContractName = 
                Prelude.Optional(options)
                    .Map(p => p.IgnoreContractName) || endpointConfig.TryGet<IgnoreContractName>().Map(p =>  p.Value);

            return transport.Subscribe(endpointConfig, Handler, options);

            async Task<Acknowledge> Handler(Serialized<byte[]> msg, EventContext ctx, CancellationToken token)
            {
                try
                {
                    var contractTypeResult = resolver.TryMap(msg.TypeCode, typeof(TEvent)).Try();

                    
                    if (!contractTypeResult.IsFaulted || ignoreContractName.IfNone(false))
                    {
                        var type = contractTypeResult.IfFail(typeof(TEvent));
                        var obj = deserialize(type, msg).Try().Unwrap();
                        if (obj is TEvent evt)
                        {
                            await eventHandler.Handle(evt, ctx, token);
                            
                        }
                        else
                            throw new NackException($"Invalid data type arrived {obj?.GetType()}");
                    }
                    else
                    {
                        contractTypeResult.Unwrap();
                    }
                    return Acknowledge.Ack;
                }
                catch (Exception ex)
                {
                    _logger.LogError(0, ex, "On recive {selector} {service}", selector, typeof(TService));
                    return exceptionPolicy.WhenException(ex);
                }
            }
               
        }

        
    }
}