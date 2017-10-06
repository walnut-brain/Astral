using System;
using System.Net.Mime;
using Astral.Links;
using RabbitLink.Consumer;

namespace RabbitLink.Services
{
    public interface IRequestEndpoint<TService, TRequest, TResponse> : IEventConsumer<TService, Response<TResponse>>, IEventPublisher<TService, Request<TRequest>>
    {
        ContentType ContentType { get; }
        bool ExchangePassive();
        IRequestEndpoint<TService, TResponse, TRequest> ExchangePassive(bool value);
        string ExchangeNamed();
        IRequestEndpoint<TService, TResponse, TRequest> ExchangeNamed(string value);
        bool ConfirmsMode();
        IRequestEndpoint<TService, TResponse, TRequest> ConfirmsMode(bool value);
        string QueueName();
        IRequestEndpoint<TService, TResponse, TRequest> QueueName(string value);
        ushort PrefetchCount();
        IRequestEndpoint<TService, TResponse, TRequest> PrefetchCount(ushort value);
        bool AutoAck();
        IRequestEndpoint<TService, TResponse, TRequest> AutoAck(bool value);
        ILinkConsumerErrorStrategy ErrorStrategy();
        IRequestEndpoint<TService, TResponse, TRequest> ErrorStrategy(ILinkConsumerErrorStrategy value);
        bool? CancelOnHaFailover();
        IRequestEndpoint<TService, TResponse, TRequest> CancelOnHaFailover(bool? value);
        bool Exclusive();
        IRequestEndpoint<TService, TResponse, TRequest> Exclusive(bool value);
        bool QueuePassive();
        IRequestEndpoint<TService, TResponse, TRequest> QueuePassive(bool value);
        bool Bind();
        IRequestEndpoint<TService, TResponse, TRequest> Bind(bool value);
        QueueParameters QueueParameters();
        IRequestEndpoint<TService, TResponse, TRequest> QueueParameters(Func<QueueParameters, QueueParameters> setter);
    }
}