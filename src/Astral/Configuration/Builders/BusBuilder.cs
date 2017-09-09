using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using Astral.Configuration.Settings;
using Astral.Exceptions;
using Astral.Payloads.DataContracts;
using Astral.Payloads.Serialization;
using Astral.Specifications;
using Astral.Transport;
using Astral.Utils;
using Lawium;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Astral.Configuration.Builders
{
    public class BusBuilder : BuilderBase
    {
        private TypeEncoding _typeEncoding = TypeEncoding.Default;

        private Serialization<byte[]> _serialization = Serialization.JsonRaw;

        private IServiceProvider ServiceProvider { get; }

        private readonly Dictionary<(string Tag, bool IsFull), DisposableValue<IRpcTransport>>
            _transports = new Dictionary<(string Tag, bool IsFull), DisposableValue<IRpcTransport>>();


        internal BusBuilder(IServiceProvider provider, string systemName) : base(new LawBookBuilder<Fact>(provider.GetService<ILoggerFactory>()))
        {
            if (systemName == null) throw new ArgumentNullException(nameof(systemName));
            BookBuilder.RegisterLaw(Law<Fact>.Axiom(new SystemName(systemName)));
            ServiceProvider = provider;
        }

        public BusBuilder SetTypeEncoding(TypeEncoding encodingProvider)
        {
            _typeEncoding = encodingProvider ?? TypeEncoding.Default;
            return this;
        }

        public BusBuilder SetSerializer(Serialization<byte[]> serialization)
        {
            _serialization = serialization ?? throw new ArgumentNullException(nameof(serialization));
            return this;
        }

        private void CleanUpTransport(string tag, bool isFull)
        {
            if (tag == null) throw new ArgumentNullException(nameof(tag));
            if (_transports.TryGetValue((tag, isFull), out var current))
            {
                current.Dispose();
                _transports.Remove((tag, isFull));
            }
        }
        
        /// <summary>
        /// Add rpc transport as instance
        /// </summary>
        /// <param name="transport">transport</param>
        /// <param name="tag">transport tag, null for default</param>
        /// <param name="isOwned">bus control lifetime transport instance</param>
        /// <returns>self</returns>
        public BusBuilder AddRpcTransport(IRpcTransport transport, string tag = null, bool isOwned  = true)
        {
            tag = ConfigUtils.NormalizeTag(tag);
            CleanUpTransport(tag, false);
            _transports[(tag, false)] = new DisposableValue<IRpcTransport>(transport, isOwned);
            return this;
        }

        /// <summary>
        /// Add rpc transport as factory
        /// </summary>
        /// <param name="transportFactory">transportFactory</param>
        /// <param name="tag">transport tag, null for default</param>
        /// <param name="isOwned">bus control lifetime transport instance</param>
        /// <returns>self</returns>
        public BusBuilder AddRpcTransport(Func<IRpcTransport> transportFactory, string tag = null, bool isOwned = true)
        {
            tag = ConfigUtils.NormalizeTag(tag);
            CleanUpTransport(tag, false);
            _transports[(tag, false)] = new DisposableValue<IRpcTransport>(new Lazy<IRpcTransport>(transportFactory), isOwned); 
            return this;
        }

        /// <summary>
        /// Add rpc transport from IoC
        /// </summary>
        /// <typeparam name="T">type to resolve from IoC</typeparam>
        /// <param name="tag">trnasport tag, null for default</param>
        /// <returns>self</returns>
        public BusBuilder AddRpcTransport<T>(string tag = null)
            where T : IRpcTransport
        {
            tag = ConfigUtils.NormalizeTag(tag);
            CleanUpTransport(tag, false);
            _transports[(tag, false)] = new DisposableValue<IRpcTransport>(new Lazy<IRpcTransport>(() => ServiceProvider.GetRequiredService<T>()), false);
            return this;
        }


        /// <summary>
        /// Add full transport as instance
        /// </summary>
        /// <param name="transport">transport</param>
        /// <param name="tag">transport tag, null for default</param>
        /// <param name="isOwned">bus control lifetime transport instance</param>
        /// <returns>self</returns>
        public BusBuilder AddTransport(ITransport transport, string tag = null, bool isOwned = true)
        {
            tag = ConfigUtils.NormalizeTag(tag);
            CleanUpTransport(tag, false);
            _transports[(tag, false)] = new DisposableValue<IRpcTransport>(transport, isOwned);
            return this;
        }

        /// <summary>
        /// Add full transport as factory
        /// </summary>
        /// <param name="transportFactory">transportFactory</param>
        /// <param name="tag">transport tag, null for default</param>
        /// <param name="isOwned">bus control lifetime transport instance</param>
        /// <returns>self</returns>
        public BusBuilder AddTransport(Func<ITransport> transportFactory, string tag = null, bool isOwned = true)
        {
            tag = ConfigUtils.NormalizeTag(tag);
            CleanUpTransport(tag, false);
            _transports[(tag, false)] = new DisposableValue<IRpcTransport>(new Lazy<IRpcTransport>(transportFactory), isOwned);
            return this;
        }

        /// <summary>
        /// Add full transport from IoC
        /// </summary>
        /// <typeparam name="T">type to resolve from IoC</typeparam>
        /// <param name="tag">trnasport tag, null for default</param>
        /// <returns>self</returns>
        public BusBuilder AddTransport<T>(string tag = null)
            where T : ITransport
        {
            tag = ConfigUtils.NormalizeTag(tag);
            CleanUpTransport(tag, false);
            _transports[(tag, false)] = new DisposableValue<IRpcTransport>(new Lazy<IRpcTransport>(() => ServiceProvider.GetRequiredService<T>()), false);
            return this;
        }


        public ServiceBuilder<TService> Service<TService>()
        {
            var type = typeof(TService);
            if (!type.IsInterface)
                throw new ArgumentException($"{type} must be interface");
            var builder = BookBuilder.GetSubBookBuilder(type, b => b.AddServiceLaws(type));
            return new ServiceBuilder<TService>(builder);
        }

        internal BusSpecification Build()
        {
            if(_transports.Count == 0)
                throw new InvalidConfigurationException("No transport configured");
            BookBuilder.RegisterLaw(Law<Fact>.Axiom(new InstanceCode(Guid.NewGuid().ToString("D"))));

            var transportProvider = new TransportProvider(new ReadOnlyDictionary<(string, bool), DisposableValue<IRpcTransport>>(_transports));
            return new BusSpecification(BookBuilder.Build(), _typeEncoding, _serialization, transportProvider, ServiceProvider);
        }
    }
}