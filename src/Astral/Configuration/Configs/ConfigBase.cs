using LanguageExt;
using Lawium;
using Microsoft.Extensions.Logging;

namespace Astral.Configuration.Configs
{
    public class ConfigBase
    {
        public ConfigBase(LawBook lawBook)
        {
            LawBook = lawBook;
            Logger = LoggerFactory.CreateLogger(GetType());
        }

        protected LawBook LawBook { get; }
        public ILoggerFactory LoggerFactory => LawBook.LoggerFactory;
        protected ILogger Logger { get; }

        public Option<T> TryGet<T>()
        {
            return LawBook.TryGet<T>();
        }
    }
}