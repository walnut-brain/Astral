using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Astral.Data;
using Astral.Deliveries;

namespace Astral.Internals
{
    internal class Responder<TService, TRequest> : IResponder where TService : class
    {
        private readonly IBusService<TService> _busService;
        private readonly Expression<Func<TService, ICall<TRequest>>> _selector;
        private readonly ChannelKind.ReplyChannel _channel;

        public Responder(IBusService<TService> busService, Expression<Func<TService, ICall<TRequest>>> selector, ChannelKind.ReplyChannel channel)
        {
            _busService = busService;
            _selector = selector;
            _channel = channel;
        }

        public Task Send(CancellationToken token)
            => _channel == null ? Task.CompletedTask : _busService.Response(_selector, _channel, token);

        public Task Send(RequestFault fault, CancellationToken token)
            => _channel == null ? Task.CompletedTask : _busService.ResponseFault(_selector, fault, _channel, token);

        public Task<Guid> Deliver<TStore>(IUnitOfWork<TStore> uow)
            => _channel == null ? Task.FromResult(Guid.Empty) : _busService.DeliverResponse(uow, _selector, _channel);

        public Task<Guid> Deliver<TStore>(IUnitOfWork<TStore> uow, RequestFault fault) 
            => _channel == null ? Task.FromResult(Guid.Empty) : _busService.DeliverFaultReply(uow, _selector, fault, _channel);
    }

    internal class Responder<TService, TRequest, TResponse> : IResponder<TResponse> where TService : class
    {
        private readonly IBusService<TService> _busService;
        private readonly Expression<Func<TService, ICall<TRequest, TResponse>>> _selector;
        private readonly ChannelKind.ReplyChannel _channel;

        public Responder(IBusService<TService> busService, Expression<Func<TService, ICall<TRequest, TResponse>>> selector, ChannelKind.ReplyChannel channel)
        {
            _busService = busService;
            _selector = selector;
            _channel = channel;
        }

        public Task Send(TResponse response, CancellationToken token)
            => _channel == null ? Task.CompletedTask : _busService.Response(_selector, response, _channel, token);

        public Task Send(RequestFault fault, CancellationToken token)
            => _channel == null ? Task.CompletedTask : _busService.ResponseFault(_selector, fault, _channel, token);

        public Task<Guid> Deliver<TStore>(IUnitOfWork<TStore> uow, TResponse response)
            
            => _channel == null ? Task.FromResult(Guid.Empty) : _busService.DeliverResponse(uow, _selector, response, _channel);

        public Task<Guid> Deliver<TStore>(IUnitOfWork<TStore> uow, RequestFault fault)
            => _channel == null ? Task.FromResult(Guid.Empty) : _busService.DeliverFaultReply(uow, _selector, fault, _channel);
    }
}