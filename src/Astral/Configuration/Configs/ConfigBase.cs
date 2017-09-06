using System;
using Astral.Configuration.Settings;
using FunEx;
using Lawium;
using Microsoft.Extensions.Logging;

namespace Astral.Configuration.Configs
{
    public class ConfigBase
    {
        public ConfigBase(LawBook lawBook, IServiceProvider provider)
        {
            LawBook = lawBook;
            Provider = provider;
            Logger = LoggerFactory.CreateLogger(GetType());
        }

        protected LawBook LawBook { get; }
        public IServiceProvider Provider { get; }
        public ILoggerFactory LoggerFactory => LawBook.LoggerFactory;
        protected ILogger Logger { get; }

        public Option<T> TryGet<T>()
        {
            return LawBook.TryGet<T>();
        }

        public string SystemName => this.Get<SystemName>().Value;

    }
}