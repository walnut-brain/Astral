using System;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Astral.Configuration;
using Astral.Configuration.Configs;
using Astral.Configuration.Settings;
using Astral.Payloads;
using Astral.Transport;
using FunEx;
using Microsoft.Extensions.Logging;

namespace Astral.Internals
{
    internal static class Operations
    {
        
        /*
        internal static IDisposable ListenEvent<TEvent>(ILogger logger, EndpointConfig config,
            RawMessageSubscribe  subscribe, 
            IEventListener<TEvent> eventListener,
            EventListenOptions options = null)
        {
            return logger.LogActivity(Listen, "listen {service} {endpoint}", config.ServiceType,
                config.PropertyInfo.Name);

            IDisposable Listen()
            {
                var exceptionPolicy = config.AsTry<IReciveExceptionPolicy>().RecoverTo(new DefaultExceptionPolicy());
                                

                return subscribe(config, (msg, ctx, token) => Listener(msg, ctx, token, exceptionPolicy), options);
            }

            async Task<Acknowledge> Listener(
                Payload<byte[]> msg, EventContext ctx, CancellationToken token, IReciveExceptionPolicy exceptionPolicy)
            {
                async Task<Acknowledge> Receive()
                {
                    var obj = Payload.FromPayload(msg, new FromPayloadOptions<byte[]>(config.TypeEncoding.ToType, 
                        config.Serializer.Deserialize)).As<TEvent>().Unwrap();

                    await eventListener.Handle(obj, ctx, token);
                    return Acknowledge.Ack;
                }

                return await Receive()
                    .LogResult(logger, "recive event {service} {endpoint}", config.ServiceType, config.PropertyInfo)
                    .CorrectError(exceptionPolicy.WhenException);
            }
        }


        public static Action EnqueueManual<TStore, TEvent>(ILogger logger, IDeliveryDataService<TStore> dataService,
            EndpointConfig config, PreparePublish<TEvent> preparePublish, TEvent @event, DeliveryManager<TStore> manager,
            EventPublishOptions options = null)
            where TStore : IStore<TStore>
        {
            
            var serialized = config.TextSerialize(@event).IfFailThrow();
            var reserveTime = config.AsTry<DeliveryReserveTime>().Map(p => p.Value).IfFail(TimeSpan.FromSeconds(3));
            var deliveryId = Guid.NewGuid();
            var key = config.TryGet<IMessageKeyExtractor<TEvent>>().Map(p => p.ExtractKey(@event)) ||
                      Prelude.Optional(@event as IKeyProvider).Map(p => p.Key);
            var serviceName = config.ServiceName;
            var endpointName = config.EndpointName;
            
            var messageTtl = options?.EventTtl ??
                             config.AsTry<MessageTtl>().Map(p => p.Value).IfFail(Timeout.InfiniteTimeSpan);

            key.Filter(_ => config.AsTry<CleanSameKeyDelivery>().Map(p => p.Value).IfFail(true))
                .IfSome(k => dataService.Where(p => p.ServiceName == serviceName && p.EndpointName == endpointName && p.Key == k).Delete().Wait());

            var deliveryRecord = new DeliveryRecord(deliveryId,
                serviceName,
                endpointName,
                serialized,
                DateTimeOffset.Now + reserveTime)
            {
                Key = key.IfNoneUnsafe((string)null),
                Ttl = messageTtl < TimeSpan.Zero ? (DateTimeOffset?)null : DateTimeOffset.Now + messageTtl
            };
              
            dataService.Insert(deliveryRecord);

            return Deliver(logger, config, deliveryRecord, serialized, @event, preparePublish,
                new PublishOptions(messageTtl, ResponseTo.None, null), manager);
        }
        
        private static Action Deliver<T, TStore>(ILogger logger, EndpointConfig config,
            DeliveryRecord record,
            PayloadBase<string> payload,
            T message,
            PreparePublish<T> preparePublish,
            PublishOptions options,
            DeliveryManager<TStore> manager)
            where TStore : IStore<TStore>
            => () =>
            {

                var defPolicy = Policy.NoOpAsync();
                    
                var policy = config.TryGet<DeliveryExceptionPolicy>().Map(p => p.Value).IfNone(defPolicy);
                var afterDelivery = 
                    record.IsAnswer ?
                        config.TryGet<AfterDelivery>().Map(p => p.Value).IfNone(OnDeliverySuccess.Delete)
                        : config.TryGet<AfterResponseDelivery>().Map(p => p.Value).IfNone(OnDeliverySuccess.Delete);

                using (logger.BeginScope("Delivery {service} {endpoint} {isAnswer}", config.ServiceType,
                    config.PropertyInfo.Name,
                    record.IsAnswer))
                {
                    try
                    {
                        var rawSerialized = config.RawSerialize(message, payload).IfFailThrow();

                        
#pragma warning disable 4014
                        manager.AddDelivery(record.DeliveryId,
                            ctk => policy.ExecuteAsync(
                                ctk1 => preparePublish(config, message, rawSerialized, options)(), ctk),
                            afterDelivery).LogResult(logger, "When delivery");
#pragma warning restore 4014
                                                    

                    }
                    catch (Exception ex)
                    {
                        logger.LogError(0, ex, "When deliver start");
                    }
                }
            };*/
    }
}