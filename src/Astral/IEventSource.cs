using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Astral.Core;
using Astral.Data;

namespace Astral
{
    public interface IEventSource<TService> //: IHasLogger
    {
        Task PublishAsync<TEvent>(Expression<Func<TService, IEvent<TEvent>>> selector, TEvent @event,
            EventPublishOptions options = null);

        /*void Publish<TEvent>(Expression<Func<TService, IEvent<TEvent>>> selector, TEvent @event,
            EventPublishOptions options = null);

        Action Enqueue<TUoW, TEvent>(IDeliveryDataService<TUoW> dataService, Expression<Func<TService, IEvent<TEvent>>> selector,
            TEvent @event,
            EventPublishOptions options = null)
            where TUoW : IUnitOfWork;*/

    }
}