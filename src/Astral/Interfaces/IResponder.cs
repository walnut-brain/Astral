using System;
using System.Threading;
using System.Threading.Tasks;
using Astral.Data;
using Astral.Deliveries;

namespace Astral
{
    public interface IResponder<in TResponse>
    {
        Task Send(TResponse response, CancellationToken token);
        
        Task Send(RequestFault fault, CancellationToken token);

        Task<Guid> Deliver<TStore>(TStore store, TResponse response)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>;
        
        Task<Guid> Deliver<TStore>(TStore store, RequestFault fault)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>;
        
    }

    public interface IResponder
    {
        Task Send(CancellationToken token);
        
        Task Send(RequestFault fault, CancellationToken token);

        Task<Guid> Deliver<TStore>(TStore store)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>;
        
        Task<Guid> Deliver<TStore>(TStore store, RequestFault fault)
            where TStore : IBoundDeliveryStore<TStore>, IStore<TStore>;
    }
}