using System.Reactive.PlatformServices;
using Astral.Lawium;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace Astral.Configuration.Configs
{
    public class ConfigBase
    {
        protected LawBook LawBook { get; }
        public ILoggerFactory LoggerFactory => LawBook.LoggerFactory;
        protected ILogger Logger { get; }

        public ConfigBase(LawBook lawBook)
        {
            LawBook = lawBook;
            Logger = LoggerFactory.CreateLogger(GetType());
        }

        public Option<T> GetOption<T>() => LawBook.TryGet<T>();
    }
}