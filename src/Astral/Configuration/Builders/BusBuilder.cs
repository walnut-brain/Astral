using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using Astral.Configuration.Configs;
using Astral.Configuration.Settings;
using Astral.Exceptions;
using Astral.Payloads.DataContracts;
using Astral.Payloads.Serialization;
using Astral.Transport;
using Lawium;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Astral.Configuration.Builders
{
    public class BusBuilder : BuilderBase
    {
        private WellKnownTypes _wellKnownTypes = WellKnownTypes.Default;
        private Func<WellKnownTypes, TypeEncoding> _typeEncoding = TypeEncoding.Default;

        private Serializer<byte[]> _serializer = new Serializer<byte[]>(Serialization.JsonRawSerializeProvider(new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        }), Serialization.JsonRawDeserializeProvider(new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        }));

        

        private readonly Dictionary<(string Tag, bool IsFull), (bool NeedIoc, bool Owned, bool Precreated, Func<IServiceProvider, IRpcTransport> Factory)>
            _transports = new Dictionary<(string, bool), (bool, bool, bool, Func<IServiceProvider, IRpcTransport>)>();
        
        
        
        public BusBuilder(string systemName, ILoggerFactory loggerFactory = null) : base(
            loggerFactory, new LawBookBuilder(loggerFactory))
        {
            if (systemName == null) throw new ArgumentNullException(nameof(systemName));
            BookBuilder.RegisterLaw(Law.Axiom(new SystemName(systemName)));
        }

        public BusBuilder SetWellKnownTypes(WellKnownTypes wellKnownTypes)
        {
            _wellKnownTypes = wellKnownTypes ?? WellKnownTypes.Default;
            return this;
        }

        public BusBuilder SetTypeEncoding(Func<WellKnownTypes, TypeEncoding> encodingProvider)
        {
            _typeEncoding = encodingProvider ?? Payloads.DataContracts.TypeEncoding.Default;
            return this;
        }

        public BusBuilder SetSerializer(Serializer<byte[]> serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            return this;
        }

        private void CleanUpTransport(string tag, bool isFull)
        {
            if (tag == null) throw new ArgumentNullException(nameof(tag));
            if(_transports.TryGetValue((tag, isFull), out var current))
                if(current.Owned && current.Precreated)
                    if(current.Factory(null) is IDisposable d) d.Dispose();  
        }
        
        public BusBuilder AddRpcTransport(IRpcTransport transport, string tag = null)
        {
            tag = NormalizeTag(tag);
            CleanUpTransport(tag, false);
            _transports[(tag, false)] = (false, true, true, _ => transport);
            return this;
        }

        public BusBuilder AddRpcTransport(Func<IRpcTransport> transportFactory, string tag = null)
        {
            tag = NormalizeTag(tag);
            CleanUpTransport(tag, false);
            _transports[(tag, false)] = (false, true, false, _ => transportFactory()); 
            return this;
        }

        public BusBuilder AddRpcTransport<T>(string tag = null)
            where T : IRpcTransport
            => AddRpcTransport(p => p.GetRequiredService<T>(), false, tag);

        public BusBuilder AddRpcTransport(Func<IServiceProvider, IRpcTransport> transportFactory, bool owned = false, string tag = null)
        {
            tag = NormalizeTag(tag);
            CleanUpTransport(tag, false);
            _transports[(tag, false)] = (true, owned, false, transportFactory);
            return this;
        }
        
        public BusBuilder AddTransport(ITransport transport, string tag = null)
        {
            tag = NormalizeTag(tag);
            CleanUpTransport(tag, true);
            _transports[(tag, true)] = (false, true, true, _ => transport);
            return this;
        }

        public BusBuilder AddTransport(Func<ITransport> transportFactory, string tag = null)
        {
            tag = NormalizeTag(tag);
            CleanUpTransport(tag, true);
            _transports[(tag, true)] = (false, true, false, _ => transportFactory()); 
            return this;
        }

        public BusBuilder AddTransport<T>(string tag = null)
            where T : ITransport
            => AddTransport(p => p.GetRequiredService<T>(), false, tag);

        public BusBuilder AddTransport(Func<IServiceProvider, ITransport> transportFactory, bool owned = false, string tag = null)
        {
            tag = NormalizeTag(tag);
            CleanUpTransport(tag, true);
            _transports[(tag, true)] = (false, owned, false, transportFactory); 
            return this;
        }

        public bool RequireIoc => _transports.Values.Any(p => p.NeedIoc);

        public ServiceBuilder<TService> Service<TService>()
        {
            var type = typeof(TService);
            if (!type.IsInterface)
                throw new ArgumentException($"{type} must be interface");
            var builder = BookBuilder.GetSubBookBuilder(type, b => b.AddServiceLaws(type));
            return new ServiceBuilder<TService>(LoggerFactory, builder);
        }

        public IBus Build(IServiceProvider serviceProvider = null)
        {
            return Build(serviceProvider, (lf, cfg) => new Bus(lf, cfg));
        }

        public TBus Build<TBus>(IServiceProvider serviceProvider,
            Func<ILoggerFactory, BusConfig, TBus> busFactory)
            where TBus : Bus
           
        {
            if(RequireIoc && serviceProvider == null)
                throw new InvalidConfigurationException("Bus required ioc for work");
            if(_transports.Count == 0)
                throw new InvalidConfigurationException("No transport configured");
            BookBuilder.RegisterLaw(Law.Axiom(new InstanceCode(Guid.NewGuid().ToString("D"))));

            TransportProvider transportProvider;
            if (serviceProvider != null)
            {
                var scope = serviceProvider.CreateScope();
                var ip = scope.ServiceProvider;
                transportProvider = new TransportProvider(scope, 
                    _transports.ToDictionary(p => p.Key, p => (p.Value.Owned, p.Value.Precreated, new Lazy<IRpcTransport>(() => p.Value.Factory(ip), LazyThreadSafetyMode.ExecutionAndPublication))));
            }
            else
                transportProvider = new TransportProvider(Disposable.Empty, 
                    _transports.ToDictionary(p => p.Key, p => (p.Value.Owned, p.Value.Precreated, new Lazy<IRpcTransport>(() => p.Value.Factory(null), LazyThreadSafetyMode.ExecutionAndPublication))));
            return busFactory(serviceProvider?.GetService<ILoggerFactory>() ?? BookBuilder.LoggerFactory , new BusConfig(BookBuilder.Build(),
                _typeEncoding(_wellKnownTypes), _serializer, 
                transportProvider));
        }

        internal static string NormalizeTag(string tag)
        {
            return string.IsNullOrWhiteSpace(tag) ? "" : tag;
        }
    }
}