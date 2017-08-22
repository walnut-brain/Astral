using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Astral;
using Astral.Configuration;
using Astral.Configuration.Settings;
using Astral.Core;
using Astral.DataContracts;
using Astral.Exceptions;
using Astral.Serialization;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace Astral
{
    public abstract class ServiceBase<TService> : IEventSource<TService>, IEventSubscriber<TService>
    {
        private readonly ServiceConfig<TService> _serviceConfig;
        private readonly IServiceProvider _serviceProvider;

        protected ServiceBase(ServiceConfig<TService> serviceConfig, IServiceProvider serviceProvider)
        {
            _serviceConfig = serviceConfig ?? throw new ArgumentNullException(nameof(serviceConfig));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        protected abstract Func<Task> PreparePublish<TEvent>(EndpointConfig config, TEvent @event,
            Serialized<byte[]> serialized, EventPublishOptions options);

        public Task PublishAsync<TEvent>(Expression<Func<TService, IEvent<TEvent>>> selector, TEvent @event, EventPublishOptions options = null)
        {
            var endpointConfig = _serviceConfig.Endpoint(selector);
            var typeToContract = endpointConfig.Get<ITypeToContractName>();
            var contractName = typeToContract.Map(typeof(TEvent), @event);
            var useMapper = endpointConfig.TryGet<UseSerializeMapper>().IfNone(() => UseSerializeMapper.Allow);
            var mapperOpt = endpointConfig.TryGet<ISerializedMapper<string, byte[]>>();
            options = new EventPublishOptions(
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
            var prepared = PreparePublish(endpointConfig, @event, serialized, options);
            return prepared();
        }

        protected abstract IDisposable Subscribe(EndpointConfig config, 
            Func<Serialized<byte[]> , EventContext, CancellationToken, Task<Acknowledge>> handler,  
            EventSubscribeOptions options);
        

        public IDisposable Subscribe<TEvent>(Expression<Func<TService, IEvent<TEvent>>> selector, 
            IEventHandler<TEvent> eventHandler, 
            EventSubscribeOptions options = null)
        {
            var endpointConfig = _serviceConfig.Endpoint(selector);
            var exceptionPolicy = endpointConfig.TryGet<IReciveExceptionPolicy>().IfNone(new DefaultExceptionPolicy());
            var logger = endpointConfig.GetLogger<ServiceBase<TService>>();
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

            return Subscribe(endpointConfig, Handler, options);

            async Task<Acknowledge> Handler(Serialized<byte[]> msg, EventContext ctx, CancellationToken token)
            {
                try
                {
                    var contractTypeResult = resolver.TryMap(msg.TypeCode, typeof(TEvent));
                    
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
                    logger.LogError(0, ex, "On recive {selector} {service}", selector, typeof(TService));
                    return exceptionPolicy.WhenException(ex);
                }
            }
               
        }

        
    }
}