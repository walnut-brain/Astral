using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Astral.Data;
using Astral.Deliveries;

namespace Astral
{
    public interface IEventSource<TService>
    {
        Task PublishAsync<TEvent>(Expression<Func<TService, IEvent<TEvent>>> selector, TEvent @event,
            EventPublishOptions options = null);


        Action EnqueueManual<TStore, TEvent>(IDeliveryDataService<TStore> dataService,
            Expression<Func<TService, IEvent<TEvent>>> selector,
            TEvent @event,
            EventPublishOptions options = null)
            where TStore : IStore<TStore>;
    }
}