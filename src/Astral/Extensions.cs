using System;
using System.Linq.Expressions;
using Astral.Configuration;
using Astral.Core;
using Astral.Data;
using Microsoft.Extensions.Logging;
using Polly;
using WalnutBrain.Data;

namespace Astral
{
    public static class Extensions
    {
        public static void Publish<TService, TEvent>(this IEventSource<TService> source,
            Expression<Func<TService, IEvent<TEvent>>> selector, TEvent @event,
            EventPublishOptions options = null)
        {
            var policy = source.GetConfig(selector).Get<EventPublishPolicy>()
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

        
        public static void EnqueueSend<TService, TUoW, TEvent>(this IEventSource<TService> source,
            IDeliveryDataService<TUoW> dataService, Expression<Func<TService, IEvent<TEvent>>> selector,
            TEvent @event,
            EventPublishOptions options = null)
            where TUoW : IUnitOfWork, IRegisterAfterCommit
        {
            var action = source.Enqueue(dataService, selector, @event, options);
            dataService.UnitOfWork.RegisterAfterCommit(action);
        }
    }
}