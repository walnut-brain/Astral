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
using Astral.Delivery;
using Astral.DependencyInjection;
using Astral.Exceptions;
using Astral.Serialization;
using Astral.Transport;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;

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
            var serialized = endpointConfig.RawSerialize(@event).IfFailThrow();
            
            var poptions = new PublishOptions(
                options?.EventTtl ?? endpointConfig.TryGet<MessageTtl>().Map(p => p.Value).IfFail(Timeout.InfiniteTimeSpan));
            
            var prepared = transport.PreparePublish(endpointConfig, @event, serialized, poptions);
            return prepared();
        }

        public Action EnqueueManual<TUoW, TEvent>(IDeliveryDataService<TUoW> dataService, Expression<Func<TService, IEvent<TEvent>>> selector, TEvent @event,
            EventPublishOptions options = null) where TUoW : IUnitOfWork
        {
            var endpointConfig = _serviceConfig.Endpoint(selector);
            var serialized = endpointConfig.TextSerialize(@event).IfFailThrow();
            var reserveTime = endpointConfig.TryGet<DeliveryReserveTime>().Map(p => p.Value).IfFail(TimeSpan.FromSeconds(3));
            var deliveryId = Guid.NewGuid();
            var key = endpointConfig.TryGet<IMessageKeyExtractor<TEvent>>().Map(p => p.ExtractKey(@event)).ToOption() ||
                      Prelude.Optional(@event as IKeyProvider).Map(p => p.Key);
            var serviceName = endpointConfig.ServiceName;
            var endpointName = endpointConfig.EndpointName;
            key.Filter(_ => endpointConfig.TryGet<CleanSameKeyDelivery>().Map(p => p.Value).IfFail(true))
                .IfSome(k => dataService.DeleteAll(serviceName, endpointName, k));
            var messageTtl = options?.EventTtl ??
                             endpointConfig.TryGet<MessageTtl>().Map(p => p.Value).IfFail(Timeout.InfiniteTimeSpan);



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

        private static Action Deliver<T, TUoW>(ILogger logger, EndpointConfig config,
            DeliveryRecord record,
            Serialized<string> serialized,
            T message,
            IMqTransport transport,
            PublishOptions options,
            DeliveryManager<TUoW> manager)
            where TUoW : IUnitOfWork
            => () =>
            {
                using (logger.BeginScope("Delivery {service} {endpoint} {isAnswer}", record.ServiceName,
                    record.EndpointName,
                    record.IsAnswer))
                {
                    try
                    {
                        var rawSerialized = config.RawSerialize(message, serialized).IfFailThrow();
                        var lease = manager.AddDelivery(record.DeliveryId).Result;
                        //TODO: Configure?
                        Policy
                            .Handle<TemporaryException>()
                            //TODO: Configure
                            .WaitAndRetryAsync(5, (_, ctx) => ctx., (exception, span, ctx) => {})
                            .ExecuteAsync(token => transport.PreparePublish(config, message, rawSerialized, options)(), lease.Token)
                            .ContinueWith(tsk =>
                            {
                                if (tsk.IsCompleted)
                                {
                                    //TODO: Toconfig
                                    lease.Release(p => p.Delete(record.DeliveryId));
                                    logger.LogTrace("Delivered {id}", record.DeliveryId);
                                }
                                else {
                                    if (!tsk.IsFaulted) return;
                                    logger.LogError(0, tsk.Exception, "On delivery {id}", record.DeliveryId);
                                    // TODO: Log error to record
                                    lease.Release(p => { });
                                }
                            });

                    }
                    catch (Exception ex)
                    {
                        logger.LogError(0, ex, "When deliver start");
                    }
                }
            };


        
        

        public IDisposable Subscribe<TEvent>(Expression<Func<TService, IEvent<TEvent>>> selector, 
            IEventHandler<TEvent> eventHandler, 
            EventSubscribeOptions options = null)
        {
            var endpointConfig = _serviceConfig.Endpoint(selector);
            var transport = GetMqTransport(endpointConfig);
            var exceptionPolicy = endpointConfig.TryGet<IReciveExceptionPolicy>().IfFail(new DefaultExceptionPolicy());
            
            var resolver = endpointConfig.Get<IContractNameToType>();

            

            var ignoreContractName = 
                Prelude.Optional(options)
                    .Map(p => p.IgnoreContractName) || endpointConfig.GetOption<IgnoreContractName>().Map(p =>  p.Value);

            var deserialize = endpointConfig.DeserializeRaw();
            
            return transport.Subscribe(endpointConfig, Handler, options);

            async Task<Acknowledge> Handler(Serialized<byte[]> msg, EventContext ctx, CancellationToken token)
            {
                try
                {
                    var contractTypeResult = resolver.TryMap(msg.TypeCode, typeof(TEvent)).Try();

                    
                    if (!contractTypeResult.IsFaulted || ignoreContractName.IfNone(false))
                    {
                        var type = contractTypeResult.IfFail(typeof(TEvent));
                        var obj = deserialize(type, msg).IfFailThrow();
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