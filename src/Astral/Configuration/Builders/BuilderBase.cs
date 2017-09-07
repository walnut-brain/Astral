using System;
using Lawium;
using Microsoft.Extensions.DependencyInjection;
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
        /// <param name="bookBuilder">law builder to use</param>
        protected BuilderBase(LawBookBuilder<Fact> bookBuilder)
        {
            Logger = LoggerFactory.CreateLogger(GetType());
            BookBuilder = bookBuilder;
        }

        /// <summary>
        ///     logger factory
        /// </summary>
        public ILoggerFactory LoggerFactory => BookBuilder.LoggerFactory;

        /// <summary>
        ///     logger
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        ///     law book builder
        /// </summary>
        protected LawBookBuilder<Fact> BookBuilder { get; }

        /// <summary>
        ///     add law to configuration
        /// </summary>
        /// <param name="law">law</param>
        public void AddLaw(Law<Fact> law) => BookBuilder.RegisterLaw(law);
    }
}