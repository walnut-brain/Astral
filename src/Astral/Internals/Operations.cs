using System;
using System.Threading;
using System.Threading.Tasks;
using Astral.Configuration;
using Astral.Configuration.Configs;
using Astral.Configuration.Settings;
using Astral.DataContracts;
using Astral.Exceptions;
using Astral.Serialization;
using Astral.Transport;
using LanguageExt;
using Microsoft.Extensions.Logging;

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
    }
}