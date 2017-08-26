using Astral.Configuration.Configs;
using Astral.Transport;
using Microsoft.Extensions.Logging;

namespace Astral
{
    public class BusService<TTransport, TService>
        where TTransport : class, ITransport
        where TService : class

    {
        public BusService(ServiceConfig<TService> config, TTransport transport, ILogger logger)
        {
            Config = config;
            Transport = transport;
            Logger = logger;
        }

        internal ServiceConfig<TService> Config { get; }
        internal TTransport Transport { get; }
        internal ILogger Logger { get; }

/*
    public Action EnqueueManual<TStore, TEvent>(IDeliveryDataService<TStore> dataService,
        Expression<Func<TService, IEvent<TEvent>>> selector, TEvent @event,
        EventPublishOptions options = null)
        where TStore : IStore<TStore>
    {
        var endpointConfig = Config.Endpoint(selector);
        var serialized = endpointConfig.TextSerialize(@event).IfFailThrow();
        var reserveTime = endpointConfig.AsTry<DeliveryReserveTime>().Map(p => p.Value).IfFail(TimeSpan.FromSeconds(3));
        var deliveryId = Guid.NewGuid();
        var key = endpointConfig.AsTry<IMessageKeyExtractor<TEvent>>().Map(p => p.ExtractKey(@event)).ToOption() ||
                  Prelude.Optional(@event as IKeyProvider).Map(p => p.Key);
        var serviceName = endpointConfig.ServiceName;
        var endpointName = endpointConfig.EndpointName;
        key.Filter(_ => endpointConfig.AsTry<CleanSameKeyDelivery>().Map(p => p.Value).IfFail(true))
            .IfSome(k => dataService.DeleteAll(serviceName, endpointName, k));
        var messageTtl = options?.EventTtl ??
                         endpointConfig.AsTry<MessageTtl>().Map(p => p.Value).IfFail(Timeout.InfiniteTimeSpan);



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

    private static Action Deliver<T, TStore>(ILogger logger, EndpointConfig config,
        DeliveryRecord record,
        Serialized<string> serialized,
        T message,
        IMqTransport transport,
        PublishOptions options,
        DeliveryManager<TStore> manager)
        where TStore : IStore<TStore>
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
                        .WaitAndRetryAsync(5, _ => TimeSpan.FromSeconds(3))
                        .ExecuteAsync(token => transport.PreparePublish(config, message, rawSerialized, options)(),
                            lease.Token)
                        .ContinueWith(tsk =>
                        {
                            if (tsk.IsCompleted)
                            {
                                //TODO: Toconfig
                                lease.Release(p => p.Delete(record.DeliveryId));
                                logger.LogTrace("Delivered {id}", record.DeliveryId);
                            }
                            else
                            {
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





    

    }
*/
    }
}