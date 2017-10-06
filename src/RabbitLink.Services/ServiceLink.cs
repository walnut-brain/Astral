﻿using System;
using System.Collections.Concurrent;
using RabbitLink.Builders;
using RabbitLink.Producer;
using RabbitLink.Services.Descriptions;
using RabbitLink.Services.Internals;

namespace RabbitLink.Services
{
    internal class ServiceLink : IServiceLink
    {
        private readonly ILink _link;
        private ConcurrentDictionary<(string, bool), ILinkProducer> _producers = new ConcurrentDictionary<(string, bool), ILinkProducer>();
        private ConcurrentDictionary<string, RpcConsumer> _consumers = new ConcurrentDictionary<string, RpcConsumer>();
        public IPayloadManager PayloadManager { get; }
        public IDescriptionFactory DescriptionFactory { get; }
        

        public ServiceLink(ILink link, IPayloadManager payloadManager, IDescriptionFactory descriptionFactory, string serviceName)
        {
            _link = link;
            PayloadManager = payloadManager;
            DescriptionFactory = descriptionFactory;
            ServiceName = serviceName;
        }

        public void Dispose()
        {
            _link.Dispose();
            foreach (var consumer in _consumers)
            {
                consumer.Value.Dispose();
            }
        }


        public void Initialize() => _link.Initialize();


        public bool IsConnected => _link.IsConnected;
        public ILinkProducerBuilder Producer => _link.Producer;
        public ILinkTopologyBuilder Topology => _link.Topology;
        public ILinkConsumerBuilder Consumer => _link.Consumer;
        public string ServiceName { get; }
        public event EventHandler Connected
        {
            add => _link.Connected += value;
            remove => _link.Connected -= value;
        }

        public event EventHandler Disconnected
        {
            add => _link.Disconnected += value;
            remove => _link.Disconnected -= value;
        }

        public IServiceBuilder<TService> Service<TService>()
            => new ServiceBuilder<TService>(DescriptionFactory.GetDescription(typeof(TService)), this);

        public ILinkProducer GetOrAddProducer(string name, bool confirmMode, Func<ILinkProducer> factory)
        {
            ILinkProducer created = null;
            var producer = _producers.GetOrAdd((name, confirmMode), _ =>
            {
                created = factory();
                return created;
            });
            if (producer != created) created?.Dispose();
            return producer;
        }

        public RpcConsumer GetOrAddConsumer(string name, Func<RpcConsumer> factory)
        {
            RpcConsumer created = null;
            var consumer = _consumers.GetOrAdd(name, _ =>
            {
                created = factory();
                return created;
            });
            if(consumer != created) created?.Dispose();
            return consumer;
        }
    }
}