using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Astral.Configuration;
using Astral.Configuration.Builders;
using Astral.Configuration.Configs;
using Astral.Configuration.Settings;
using Astral.Core;
using Astral.Data;
using Astral.DataContracts;
using Astral.Delivery;
using Astral.Exceptions;
using Astral.Serialization;
using Astral.Transport;
using LanguageExt;
using Microsoft.Extensions.Logging;
using Polly;

namespace Astral.Internals
{
    internal static class Operations
    {
        internal static Task PublishEventAsync<TEvent>(ILogger logger, EndpointConfig config, IEventTransport transport,
            TEvent @event, EventPublishOptions options = null)
        {
            Task Publish()
            {
                var serialized = config.RawSerialize(@event).IfFailThrow();

                var poptions = new PublishOptions(
                    options?.EventTtl ?? config.AsTry<MessageTtl>().Map(p => p.Value)
                        .IfFail(Timeout.InfiniteTimeSpan));

                var prepared = transport.PreparePublish(config, @event, serialized, poptions);

                return prepared();
            }

            return logger.LogActivity(Publish, "event {service} {endpoint}", config.ServiceType,
                config.PropertyInfo.Name);
        }

        internal static IDisposable ListenEvent<TEvent>(ILogger logger, EndpointConfig config,
            IEventTransport transport, IEventListener<TEvent> eventListener,
            EventListenOptions options = null)
        {
            return logger.LogActivity(Listen, "listen {service} {endpoint}", config.ServiceType,
                config.PropertyInfo.Name);

            IDisposable Listen()
            {
                var exceptionPolicy = config.AsTry<IReciveExceptionPolicy>().IfFail(new DefaultExceptionPolicy());
                var resolver = config.Get<IContractNameToType>();
                var ignoreContractName =
                    Prelude.Optional(options)
                        .Map(p => p.IgnoreContractName) || config.TryGet<IgnoreContractName>().Map(p => p.Value);

                var deserialize = config.DeserializeRaw();

                return transport.Subscribe(config, (msg, ctx, token) => Listener(msg, ctx, token, resolver,
                    ignoreContractName, deserialize, exceptionPolicy), options);
            }

            async Task<Acknowledge> Listener(
                Serialized<byte[]> msg, EventContext ctx, CancellationToken token,
                IContractNameToType resolver, Option<bool> ignoreContractName,
                Func<Type, Serialized<byte[]>, Try<object>> deserialize,
                IReciveExceptionPolicy exceptionPolicy)
            {
                async Task<Acknowledge> Receive()
                {
                    var contractTypeResult = resolver.TryMap(msg.TypeCode, typeof(TEvent).Cons()).Try();


                    if (!contractTypeResult.IsFaulted || ignoreContractName.IfNone(false))
                    {
                        var type = contractTypeResult.IfFail(typeof(TEvent));
                        var obj = deserialize(type, msg).IfFailThrow();
                        if (obj is TEvent evt)
                            await eventListener.Handle(evt, ctx, token);
                        else
                            throw new NackException($"Invalid data type arrived {obj?.GetType()}");
                    }
                    else
                    {
                        contractTypeResult.Unwrap();
                    }
                    return Acknowledge.Ack;
                }

                return await Receive()
                    .LogResult(logger, "recive event {service} {endpoint}", config.ServiceType, config.PropertyInfo)
                    .CorrectError(exceptionPolicy.WhenException);
            }
        }

        public static Action EnqueueManual<TStore, TEvent>(ILogger logger, IDeliveryDataService<TStore> dataService,
            EndpointConfig config, IEventTransport transport, TEvent @event, DeliveryManager<TStore> manager,
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

            return Deliver(logger, config, deliveryRecord, serialized, @event, transport,
                new PublishOptions(messageTtl), manager);
        }

        private static Action Deliver<T, TStore>(ILogger logger, EndpointConfig config,
            DeliveryRecord record,
            Serialized<string> serialized,
            T message,
            IEventTransport transport,
            PublishOptions options,
            DeliveryManager<TStore> manager)
            where TStore : IStore<TStore>
            => () =>
            {

                var defPolicy = Policy.NoOpAsync();
                    
                var policy = config.TryGet<DeliveryExceptionPolicy>().Map(p => p.Value).IfNone(defPolicy);
                var afterDelivery = 
                    record.IsAnswer ?
                        config.TryGet<AfterDelivery>().Map(p => p.Value).IfNone(ReleaseAction.Delete)
                        : config.TryGet<AfterAnswerDelivery>().Map(p => p.Value).IfNone(ReleaseAction.Delete);

                using (logger.BeginScope("Delivery {service} {endpoint} {isAnswer}", config.ServiceType,
                    config.PropertyInfo.Name,
                    record.IsAnswer))
                {
                    try
                    {
                        var rawSerialized = config.RawSerialize(message, serialized).IfFailThrow();
                        var lease = manager.AddDelivery(record.DeliveryId).Result;
                        policy
                            .ExecuteAsync(token => transport.PreparePublish(config, message, rawSerialized, options)(),
                                lease.Token)
                            .ContinueWith(async tsk =>
                            {
                                if (tsk.IsCompleted)
                                {
                                    await lease.Release(afterDelivery);
                                    logger.LogTrace("Delivered {id}", record.DeliveryId);
                                }
                                else
                                {
                                    if (tsk.IsFaulted)
                                    {
                                        logger.LogError(0, tsk.Exception, "On delivery {id}", record.DeliveryId);
                                        await lease.Release(ReleaseAction.Error(tsk.Exception));
                                    }
                                    else
                                        lease.Dispose();
                                }
                            });

                    }
                    catch (Exception ex)
                    {
                        logger.LogError(0, ex, "When deliver start");
                    }
                }
            };
    }
}