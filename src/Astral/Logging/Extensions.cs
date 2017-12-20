// ReSharper disable once CheckNamespace

using System;
using Astral.Logging;

namespace Astral
{
    public static partial class Extensions
    {
        public static ILog With(this ILog logger, string parameter, object value)
            => logger.With(parameter, () => value);

        public static void Trace(this ILog logger, string message, Exception ex = null)
            => logger.Write(LogLevel.Trace, message, ex);
        
        public static void Trace(this ILog logger, Exception ex = null)
            => logger.Write(LogLevel.Trace, null, ex);
        
        public static void Debug(this ILog logger, string message, Exception ex = null)
            => logger.Write(LogLevel.Debug, message, ex);
        
        public static void Debug(this ILog logger, Exception ex = null)
            => logger.Write(LogLevel.Debug, null, ex);
        
        public static void Info(this ILog logger, string message, Exception ex = null)
            => logger.Write(LogLevel.Information, message, ex);
        
        public static void Info(this ILog logger, Exception ex = null)
            => logger.Write(LogLevel.Information, null, ex);
        
        public static void Warn(this ILog logger, string message, Exception ex = null)
            => logger.Write(LogLevel.Warning, message, ex);
        
        public static void Warn(this ILog logger, Exception ex = null)
            => logger.Write(LogLevel.Warning, null, ex);
        
        public static void Error(this ILog logger, string message, Exception ex = null)
            => logger.Write(LogLevel.Error, message, ex);
        
        public static void Error(this ILog logger, Exception ex = null)
            => logger.Write(LogLevel.Error, null, ex);
        
        public static void Fatal(this ILog logger, string message, Exception ex = null)
            => logger.Write(LogLevel.Critical, message, ex);
        
        public static void Fatal(this ILog logger, Exception ex = null)
            => logger.Write(LogLevel.Critical, null, ex);
        
    }
}