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

        Task<Guid> Deliver<TStore>(IUnitOfWork<TStore> uow, TResponse response);

        Task<Guid> Deliver<TStore>(IUnitOfWork<TStore> uow, RequestFault fault);
        
    }

    public interface IResponder
    {
        Task Send(CancellationToken token);
        
        Task Send(RequestFault fault, CancellationToken token);

        Task<Guid> Deliver<TStore>(IUnitOfWork<TStore> uow);

        Task<Guid> Deliver<TStore>(IUnitOfWork<TStore> uow, RequestFault fault);
    }
}