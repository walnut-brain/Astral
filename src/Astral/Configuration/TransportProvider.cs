using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using Astral.Configuration;
using Astral.Exceptions;
using Astral.Transport;
using Astral.Utils;
using FunEx.Monads;

namespace Astral.Specifications
{
    internal class TransportProvider : IDisposable
    {
        private readonly IReadOnlyDictionary<string, DisposableValue<ITransport>> _transports;

        private readonly ICancelable _disposable;

        public TransportProvider(IReadOnlyDictionary<string, DisposableValue<ITransport>> transports)
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
                if(!_transports.TryGetValue(tag, out var rec))
                    throw new TransportNotFoundException(tag);
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