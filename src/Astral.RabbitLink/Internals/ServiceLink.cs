using System;
using System.Collections.Concurrent;
using System.Threading;
using Astral.RabbitLink.Descriptions;
using Microsoft.Extensions.Logging;
using RabbitLink;
using RabbitLink.Builders;
using RabbitLink.Producer;
using Astral.Logging;

namespace Astral.RabbitLink.Internals
{
    internal class ServiceLink : IServiceLink
    {
        private readonly ILink _link;
        private ReaderWriterLockSlim DisposeLock { get; } = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private bool _isDisposed;
        
        public ILogFactory LogFactory { get; }
        private ILog Log { get; }

        private readonly ConcurrentDictionary<(string, bool), ILinkProducer> _producers =
            new ConcurrentDictionary<(string, bool), ILinkProducer>();

        private readonly ConcurrentDictionary<string, RpcConsumer> _consumers =
            new ConcurrentDictionary<string, RpcConsumer>();
        
        public ILinkPayloadManager PayloadManager { get; }
        public IDescriptionFactory DescriptionFactory { get; }
        

        public ServiceLink(ILink link, ILinkPayloadManager linkPayloadManager, IDescriptionFactory descriptionFactory, string serviceName, ILogFactory logFactory)
        {
            _link = link ?? throw new ArgumentNullException(nameof(link));
            LogFactory = logFactory ?? throw new ArgumentNullException(nameof(logFactory));
            PayloadManager = linkPayloadManager ?? throw new ArgumentNullException(nameof(linkPayloadManager));
            DescriptionFactory = descriptionFactory ?? throw new ArgumentNullException(nameof(descriptionFactory));
            HolderName = serviceName ?? throw new ArgumentNullException(nameof(serviceName));
            Log = LogFactory.CreateLog<ServiceLink>();
        }

        private void GuardDispose(Action action)
        {
            DisposeLock.EnterReadLock();
            try
            {
                if(_isDisposed) throw new ObjectDisposedException(GetType().Name);
                action();
            }
            finally
            {
                DisposeLock.ExitReadLock();
            }
            
        }

        private T GuardDispose<T>(Func<T> action)
        {
            DisposeLock.EnterReadLock();
            try
            {
                if(_isDisposed) throw new ObjectDisposedException(GetType().Name);
                return action();
            }
            finally
            {
                DisposeLock.ExitReadLock();
            }
            
        }

        public void Dispose()
        {
            DisposeLock.EnterWriteLock();
            try
            {
                if (_isDisposed) return;
                Log.Trace("Disposing");
                _isDisposed = true;
                _link.Dispose();
                foreach (var consumer in _consumers)
                {
                    consumer.Value.Dispose();
                }
                Log.Trace("Disposed");
            }
            finally
            {
                DisposeLock.ExitWriteLock();
            }
        }


        public void Initialize() => _link.Initialize();


        public bool IsConnected => _link.IsConnected;
        public ILinkProducerBuilder Producer => _link.Producer;
        public ILinkTopologyBuilder Topology => _link.Topology;
        public ILinkConsumerBuilder Consumer => _link.Consumer;
        public string HolderName { get; }
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
            =>
                GuardDispose(() =>
                {
                    var log = Log.With("name", name).With("confirmMode", confirmMode);
                    log.Trace($"{nameof(GetOrAddProducer)} enter");
                    try
                    {
                        ILinkProducer created = null;
                        var producer = _producers.GetOrAdd((name, confirmMode), _ =>
                        {
                            created = factory();
                            return created;
                        });
                        if (producer != created) created?.Dispose();
                        log.With("created", producer == created).Trace($"{nameof(GetOrAddProducer)} success");
                        return producer;
                    }
                    catch (Exception ex)
                    {
                        log.Error($"{nameof(GetOrAddProducer)} error", ex);
                        throw;
                    }
                });


        public RpcConsumer GetOrAddConsumer(string name, Func<RpcConsumer> factory)
            => GuardDispose(() =>
            {
                var log = Log.With("name", name);
                log.Trace($"{nameof(GetOrAddConsumer)} enter");
                try
                {
                    RpcConsumer created = null;
                    var consumer = _consumers.GetOrAdd(name, _ =>
                    {
                        created = factory();
                        return created;
                    });
                    if (consumer != created) created?.Dispose();
                    log.With("created", consumer == created).Trace($"{nameof(GetOrAddConsumer)} success");
                    return consumer;
                }
                catch (Exception ex)
                {
                    log.Error($"{nameof(GetOrAddConsumer)} error", ex);
                    throw;
                }
                
            });

        ~ServiceLink()
        {
            Dispose();
        }
    }
}