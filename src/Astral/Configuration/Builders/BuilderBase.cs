using Astral.Fakes;
using Lawium;
using Microsoft.Extensions.Logging;

namespace Astral.Configuration.Builders
{
    /// <summary>
    ///     Base law builder based configuration builder
    /// </summary>
    public abstract class BuilderBase
    {
        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="loggerFactory">logger factory</param>
        /// <param name="bookBuilder">law builder to use</param>
        protected BuilderBase(ILoggerFactory loggerFactory, LawBookBuilder bookBuilder)
        {
            LoggerFactory = loggerFactory ?? new FakeLoggerFactory();
            Logger = loggerFactory.CreateLogger(GetType());
            BookBuilder = bookBuilder;
        }

        /// <summary>
        ///     logger factory
        /// </summary>
        public ILoggerFactory LoggerFactory { get; }

        /// <summary>
        ///     logger
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        ///     law book builder
        /// </summary>
        protected LawBookBuilder BookBuilder { get; }

        /// <summary>
        ///     add law to configuration
        /// </summary>
        /// <param name="law">law</param>
        public void AddLaw(Law law)
        {
            BookBuilder.RegisterLaw(law);
        }
    }
}