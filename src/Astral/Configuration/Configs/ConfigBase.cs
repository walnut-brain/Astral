using System;
using System.Data.SqlTypes;
using Astral.Configuration.Settings;
using FunEx;
using FunEx.Monads;
using Lawium;
using Microsoft.Extensions.Logging;

namespace Astral.Configuration.Configs
{
    public class ConfigBase : IServiceProvider
    {
        private readonly Func<Type, object> _provider;

        public ConfigBase(LawBook<Fact> lawBook, Func<Type, object> provider)
        {
            _provider = provider;
            LawBook = lawBook;
            Logger = LoggerFactory.CreateLogger(GetType());
        }

        protected LawBook<Fact> LawBook { get; }
        public ILoggerFactory LoggerFactory => LawBook.LoggerFactory;
        protected ILogger Logger { get; }

        public object GetService(Type serviceType) => _provider(serviceType);

        public Option<T> TryGet<T>() where T : Fact
        {
            return LawBook.TryGet<T>();
        }

        public string SystemName => this.Get<SystemName>().Value;

    }
}