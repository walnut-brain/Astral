using System;
using System.Linq.Expressions;
using Astral.Configuration;
using Astral.Configuration.Settings;
using Astral.Core;
using Astral.Data;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using WalnutBrain.Data;

namespace Astral
{
    public static class Extensions
    {
        /*
        public static void Publish<TService, TEvent>(this IEventSource<TService> source,
            Expression<Func<TService, IEvent<TEvent>>> selector, TEvent @event,
            EventPublishOptions options = null)
        {
            var policy = source.GetConfig(selector).GetOption<EventPublishPolicy>()
                .Map(p => p.Value)
                .IfNone(Astral.DefaultEventPublishPolicy);

            Action<Exception> Log(ILogger logger, Expression<Func<TService, IEvent<TEvent>>> slt, TEvent ev)
                => ex => logger.LogError(0, ex, "On publishing {endpoint} {@event}", slt, ev);

            var log = Log(source.Logger, selector, @event);
            
            policy = Policy
                .Handle<Exception>()
                .Fallback(() => { }, log)
                .Wrap(policy);
            policy.ExecuteAsync(() => source.PublishAsync(selector, @event, options));

        }

        public static Action EnqueueManual<TService, TUoW, TEvent>(this IEventSource<TService> source,
            IDeliveryDataService<TUoW> dataService, Expression<Func<TService, IEvent<TEvent>>> selector,
            TEvent @event,
            EventPublishOptions options = null)
            where TUoW : IUnitOfWork
        {
            var serviceName = source.GetConfig(selector).GetRequiredService<ServiceName>().Value;
            var endpointName = source.GetConfig(selector).GetRequiredService<EndpointName>().Value;
            var serializer = source.GetConfig(selector).GetRequiredService<DeliverySerialize>().Value;
            var strSerialized = serializer.Serialize()
            var policy = source.GetConfig(selector).GetOption<DeliveryPolicy>()
                .Map(p => p.Value)
                .IfNone(Astral.DefaultDeliveryPolicy);
            var deliveryId = Guid.NewGuid();
            dataService.Create(new DeliveryRecord(deliveryId, serviceName, endpointName, )
            {
                
            });        
        }
        
        
        public static void EnqueueSend<TService, TUoW, TEvent>(this IEventSource<TService> source,
            IDeliveryDataService<TUoW> dataService, Expression<Func<TService, IEvent<TEvent>>> selector,
            TEvent @event,
            EventPublishOptions options = null)
            where TUoW : IUnitOfWork, IRegisterAfterCommit
        {
            var action = source.Enqueue(dataService, selector, @event, options);
            dataService.UnitOfWork.RegisterAfterCommit(action);
        }

        public static Option<T> GetOption<T>(this IServiceProvider provider)
        {
            return Prelude.Optional(provider.GetService<T>());
        }

    */
    }
}