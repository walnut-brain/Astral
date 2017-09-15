﻿using System;
using Astral.Specifications;
using Lawium;
using Microsoft.Extensions.Logging;

namespace Astral.Configuration.Builders
{
    public class ChannelBuilder  
    {
        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="bookBuilder">law builder to use</param>
        internal ChannelBuilder(LawBookBuilder bookBuilder)
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
        protected LawBookBuilder BookBuilder { get; }

        /// <summary>
        ///     add law to configuration
        /// </summary>
        /// <param name="law">law</param>
        public void AddLaw(Law law) => BookBuilder.RegisterLaw(law);
    }
}