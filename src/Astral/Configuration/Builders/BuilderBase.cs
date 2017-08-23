using Astral.Fakes;
using Astral.Lawium;
using Microsoft.Extensions.Logging;

namespace Astral.Configuration.Builders
{
    public abstract class BuilderBase
    {
        public ILoggerFactory LoggerFactory { get; }

        protected ILogger Logger { get; }

        protected BuilderBase(ILoggerFactory loggerFactory, LawBookBuilder bookBuilder)
        {
            LoggerFactory = loggerFactory ?? new FakeLoggerFactory();
            Logger = loggerFactory.CreateLogger(this.GetType());
            BookBuilder = bookBuilder;
        }

        protected LawBookBuilder BookBuilder { get; }

        public void AddLaw(Law law)
            => BookBuilder.RegisterLaw(law);



    }
}