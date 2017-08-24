using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Astral.Core;
using Astral.Data;

namespace Astral
{
    public interface IEventSource<TService> 
    {
        Task PublishAsync<TEvent>(Expression<Func<TService, IEvent<TEvent>>> selector, TEvent @event,
            EventPublishOptions options = null);

        

        Action EnqueueManual<TUoW, TEvent>(IDeliveryDataService<TUoW> dataService, Expression<Func<TService, IEvent<TEvent>>> selector,
            TEvent @event,
            EventPublishOptions options = null)
            where TUoW : IUnitOfWork;

    }
}