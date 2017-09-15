using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using Astral.Configuration.Settings;
using Astral.Deliveries;
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

        private readonly Dictionary<string, DisposableValue<ITransport>>
            _transports = new Dictionary<string, DisposableValue<ITransport>>();


        internal BusBuilder(IServiceProvider provider, string systemName) : base(new LawBookBuilder(provider.GetService<ILoggerFactory>()))
        {
            if (systemName == null) throw new ArgumentNullException(nameof(systemName));
            BookBuilder.RegisterLaw(Law.Axiom(new SystemName(systemName)));
            BookBuilder.RegisterLaw(Law.Create("DeliveryOnSuccess defaults", (EndpointType et) => et == EndpointType.Event 
                ? DeliveryOnSuccess.Delete 
                : DeliveryOnSuccess.Archive));
            BookBuilder.RegisterLaw(Law.Axiom(new ResponseTo(ChannelKind.System)));
            BookBuilder.RegisterLaw(Law.Axiom(new RpcTimeout(TimeSpan.FromHours(1))));
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

        private void CleanUpTransport(string tag)
        {
            if (tag == null) throw new ArgumentNullException(nameof(tag));
            if (_transports.TryGetValue(tag, out var current))
            {
                current.Dispose();
                _transports.Remove(tag);
            }
        }
        


        /// <summary>
        /// Add transport as instance
        /// </summary>
        /// <param name="transport">transport</param>
        /// <param name="tag">transport tag, null for default</param>
        /// <param name="isOwned">bus control lifetime transport instance</param>
        /// <returns>self</returns>
        public BusBuilder AddTransport(ITransport transport, string tag = null, bool isOwned = true)
        {
            tag = ConfigUtils.NormalizeTag(tag);
            CleanUpTransport(tag);
            _transports[tag] = new DisposableValue<ITransport>(transport, isOwned);
            return this;
        }

        /// <summary>
        /// Add transport as factory
        /// </summary>
        /// <param name="transportFactory">transportFactory</param>
        /// <param name="tag">transport tag, null for default</param>
        /// <param name="isOwned">bus control lifetime transport instance</param>
        /// <returns>self</returns>
        public BusBuilder AddTransport(Func<ITransport> transportFactory, string tag = null, bool isOwned = true)
        {
            tag = ConfigUtils.NormalizeTag(tag);
            CleanUpTransport(tag);
            _transports[tag] = new DisposableValue<ITransport>(new Lazy<ITransport>(transportFactory), isOwned);
            return this;
        }

        /// <summary>
        /// Add transport from IoC
        /// </summary>
        /// <typeparam name="T">type to resolve from IoC</typeparam>
        /// <param name="tag">trnasport tag, null for default</param>
        /// <returns>self</returns>
        public BusBuilder AddTransport<T>(string tag = null)
            where T : ITransport
        {
            tag = ConfigUtils.NormalizeTag(tag);
            CleanUpTransport(tag);
            _transports[tag] = new DisposableValue<ITransport>(new Lazy<ITransport>(() => ServiceProvider.GetRequiredService<T>()), false);
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

        internal BusConfig Build()
        {
            if(_transports.Count == 0)
                throw new InvalidConfigurationException("No transport configured");
            BookBuilder.RegisterLaw(Law.Axiom(new InstanceCode(Guid.NewGuid().ToString("D"))));

            var transportProvider = new TransportProvider(new ReadOnlyDictionary<string, DisposableValue<ITransport>>(_transports));
            return new BusConfig(BookBuilder.Build(), _typeEncoding, _serialization, transportProvider, ServiceProvider);
        }
    }
}