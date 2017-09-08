using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using Astral.Configuration.Builders;
using Astral.Exceptions;
using Astral.Transport;
using Astral.Utils;
using FunEx;
using FunEx.Monads;

namespace Astral.Configuration.Configs
{
    internal class TransportProvider : IDisposable
    {
        private readonly IReadOnlyDictionary<(string, bool), DisposableValue<IRpcTransport>> _transports;

        private readonly ICancelable _disposable;

        public TransportProvider(IReadOnlyDictionary<(string, bool), DisposableValue<IRpcTransport>> transports)
        {
            _transports = transports;
            _disposable = (ICancelable) Disposable.Create(() =>
                {
                    foreach (var value in _transports.Values)
                        value.Dispose();
                });
        }

        public Result<ITransport> GetTransport(string tag = null)
        {
            tag = ConfigUtils.NormalizeTag(tag);
            return Result.Try(() =>
            {
                if(_disposable.IsDisposed)
                    throw new ObjectDisposedException(nameof(TransportProvider));
                if(!_transports.TryGetValue((tag, true), out var rec))
                    throw new TransportNotFoundException(false, tag);
                var transport = (ITransport) rec.Value;
                return transport;
            });
        }
        
        public Result<IRpcTransport> GetRpcTransport(string tag = null)
        {
            tag = ConfigUtils.NormalizeTag(tag);
            return Result.Try(() =>
            {
                if(_disposable.IsDisposed)
                    throw new ObjectDisposedException(nameof(TransportProvider));
                if(!_transports.TryGetValue((tag, false), out var rec))
                    if(!_transports.TryGetValue((tag, true), out rec))
                    throw new TransportNotFoundException(true, tag);
                var transport = rec.Value;
                return transport;
            });
        }

        public void Dispose()
        {
            if(_disposable.IsDisposed) return;
            _disposable.Dispose();
            
        }
    }
}