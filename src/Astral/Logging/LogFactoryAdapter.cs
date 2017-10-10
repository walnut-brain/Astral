using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Astral.Logging
{
    public class LogFactoryAdapter : ILogFactory
    {
        private readonly ILoggerFactory _loggerFactory;

        public LogFactoryAdapter(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public ILog CreateLog(string category)
        {
            
            return new LogAdapter(_loggerFactory.CreateLogger(category));
        }

        public ILog CreateLog<T>()
        {
            return new LogAdapter(_loggerFactory.CreateLogger<T>());
        }

        public ILog CreateLog(Type type)
        {
            return new LogAdapter(_loggerFactory.CreateLogger(type));
        }

        private class LogAdapter : ILog
        {
            private readonly string _parameter;
            private readonly Func<object> _value;
            private readonly ILogger _logger;
            private readonly LogAdapter _parent;


            public LogAdapter(ILogger logger)
            {
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }

            private LogAdapter(LogAdapter parent, string parameter, Func<object> value)
            {
                if (string.IsNullOrWhiteSpace(parameter))
                    throw new ArgumentException("Value cannot be null or whitespace.", nameof(parameter));
                _parent = parent ?? throw new ArgumentNullException(nameof(parent));
                _parameter = parameter;
                _value = value ?? throw new ArgumentNullException(nameof(value));
            }

            
            
            private void DoWrite(IDictionary<string, object> parameters, LogLevel level, string message, Exception ex)
            {
                if (parameters == null) throw new ArgumentNullException(nameof(parameters));
                if(_parameter != null)
                    parameters.Add(_parameter, _value());
                if(_parent == null)
                    using(_logger.BeginScope(parameters))
                        switch (level)
                        {
                            case LogLevel.Trace:
                                _logger.LogTrace(0, ex, message);
                                break;
                            case LogLevel.Debug:
                                _logger.LogDebug(0, ex, message);
                                break;
                            case LogLevel.Information:
                                _logger.LogInformation(0, ex, message);
                                break;
                            case LogLevel.Warning:
                                _logger.LogWarning(0, ex, message);
                                break;
                            case LogLevel.Error:
                                _logger.LogError(0, ex, message);
                                break;
                            case LogLevel.Critical:
                                _logger.LogCritical(0, ex, message);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(level), level, null);
                        }
                else
                    _parent.DoWrite(parameters, level, message, ex);
                
            }
            
            public void Write(LogLevel level, string message, Exception ex = null)
            {
                DoWrite(new Dictionary<string, object>(), level, message, ex);
            }

            public ILog With(string parameter, Func<object> value)
            {
                return new LogAdapter(this, parameter, value);
            }
        }
    }
    
    
}