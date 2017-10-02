﻿using System;
using Astral.Disposables;
using Microsoft.Extensions.Logging;

namespace RabbitLink.Astral
{
    internal class FakeLogger : ILogger
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            
        }

        public bool IsEnabled(LogLevel logLevel) => false;

        public IDisposable BeginScope<TState>(TState state)
            => Disposable.Empty;
    }
}