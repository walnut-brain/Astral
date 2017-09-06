using System;
using System.Data.SqlTypes;
using Astral.Configuration.Settings;
using FunEx;
using Lawium;
using Microsoft.Extensions.Logging;

namespace Astral.Configuration.Configs
{
    public class ConfigBase : IServiceProvider
    {
        private readonly Func<Type, object> _provider;

        public ConfigBase(LawBook lawBook, Func<Type, object> provider)
        {
            _provider = provider;
            LawBook = lawBook;
            Logger = LoggerFactory.CreateLogger(GetType());
        }

        protected LawBook LawBook { get; }
        public ILoggerFactory LoggerFactory => LawBook.LoggerFactory;
        protected ILogger Logger { get; }

        public object GetService(Type serviceType) => _provider(serviceType);

        public Option<T> TryGet<T>()
        {
            return LawBook.TryGet<T>();
        }

        public string SystemName => this.Get<SystemName>().Value;

    }
}