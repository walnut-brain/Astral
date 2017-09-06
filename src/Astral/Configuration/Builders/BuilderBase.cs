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
        protected IServiceProvider ServiceProvider { get; }

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="serviceProvider">service provider</param>
        /// <param name="bookBuilder">law builder to use</param>
        protected BuilderBase(IServiceProvider serviceProvider, LawBookBuilder bookBuilder)
        {
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            LoggerFactory = serviceProvider.GetService<ILoggerFactory>() ?? bookBuilder.LoggerFactory;
            Logger = LoggerFactory.CreateLogger(GetType());
            BookBuilder = bookBuilder;
        }

        /// <summary>
        ///     logger factory
        /// </summary>
        public ILoggerFactory LoggerFactory { get; }

        /// <summary>
        ///     logger
        /// </summary>
        public ILogger Logger { get; }

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