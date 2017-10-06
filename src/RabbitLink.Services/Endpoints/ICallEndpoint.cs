using System;
using System.Threading;
using System.Threading.Tasks;
using Astral.Links;

namespace RabbitLink.Services
{
    public interface ICallEndpoint<TService, TArg> : IActionPublisher<TService, TArg>, IActionConsumer<TService, TArg>
    {
        ushort PrefetchCount();
        ICallEndpoint<TService, TArg> PrefetchCount(ushort value);
        TimeSpan Timeout();
        ICallEndpoint<TService, TArg> Timeout(TimeSpan value);
        TimeSpan? Expires();
        ICallEndpoint<TService, TArg> Expires(TimeSpan? value);
        bool Durable();
        ICallEndpoint<TService, TArg> Durable(bool value);
        bool AutoDelete();
        ICallEndpoint<TService, TArg> AutoDelete(bool value);
    }

    
    public interface ICallEndpoint<TService, TArg, TResult> : ICallPublisher<TService, TArg, TResult>, ICallConsumer<TService, TArg, TResult>
    {
        ushort PrefetchCount();
        ICallEndpoint<TService, TArg, TResult> PrefetchCount(ushort value);
        TimeSpan Timeout();
        ICallEndpoint<TService, TArg, TResult> Timeout(TimeSpan value);
        TimeSpan? Expires();
        ICallEndpoint<TService, TArg, TResult> Expires(TimeSpan? value);
        bool Durable();
        ICallEndpoint<TService, TArg, TResult> Durable(bool value);
        bool AutoDelete();
        ICallEndpoint<TService, TArg, TResult> AutoDelete(bool value);
    }
}