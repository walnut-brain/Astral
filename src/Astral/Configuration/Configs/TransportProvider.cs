using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using Astral.Configuration.Builders;
using Astral.Exceptions;
using Astral.Transport;
using LanguageExt;

namespace Astral.Configuration.Configs
{
    internal class TransportProvider : IDisposable
    {
        private readonly IReadOnlyDictionary<(string, bool), (bool Owned, bool Precreated, Lazy<IRpcTransport> Lazy)> _transports;

        private CompositeDisposable _disposable = new CompositeDisposable();

        public TransportProvider(IDisposable disposable, IReadOnlyDictionary<(string, bool), (bool Owned, bool Precreated, Lazy<IRpcTransport> Lazy)> transports)
        {
            _transports = transports;
            _disposable.Add(disposable);
        }

        public Try<ITransport> GetTransport(string tag = null)
        {
            tag = BusBuilder.NormalizeTag(tag);
            return Prelude.Try(() =>
            {
                if(_disposable.IsDisposed)
                    throw new ObjectDisposedException(nameof(TransportProvider));
                if(!_transports.TryGetValue((tag, true), out var rec))
                    throw new TransportNotFoundException(false, tag);
                var transport = (ITransport) rec.Lazy.Value;
                return transport;
            });
        }
        
        public Try<IRpcTransport> GetRpcTransport(string tag = null)
        {
            tag = BusBuilder.NormalizeTag(tag);
            return Prelude.Try(() =>
            {
                if(_disposable.IsDisposed)
                    throw new ObjectDisposedException(nameof(TransportProvider));
                if(!_transports.TryGetValue((tag, false), out var rec))
                    if(!_transports.TryGetValue((tag, true), out rec))
                    throw new TransportNotFoundException(true, tag);
                var transport = rec.Lazy.Value;
                return transport;
            });
        }

        public void Dispose()
        {
            if(_disposable.IsDisposed) return;
            _disposable.Dispose();
            foreach (var tuple in _transports.Values)
            {
                if(!tuple.Owned) continue;
                if (tuple.Precreated)
                {
                    if(tuple.Lazy.Value is IDisposable d)
                        d.Dispose();
                }
                else
                    if(tuple.Lazy.IsValueCreated && tuple.Lazy.Value is IDisposable d)
                        d.Dispose();
            }
        }
    }
}