using System;
using Microsoft.Extensions.Logging;

namespace Astral.Extensions
{
    public static class LoggerExtensions
    {
        public static void LogActivity(this ILogger logger, Action action, string message, params object[] args)
        {
            try
            {
                logger.LogTrace("Starting - " + message, args);
                action();
                logger.LogTrace("Success - " + message, args);
            }
            catch (Exception ex)
            {
                logger.LogError(0, ex, "Error - " + message, args);
                throw;
            }
        }

        public static T LogActivity<T>(this ILogger logger, Func<T> func, string message, params object[] args)
        {
            try
            {
                logger.LogTrace("Starting - " + message, args);
                var result = func();
                logger.LogTrace("Success - " + message, args);
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(0, ex, "Error - " + message, args);
                throw;
            }
        }
    }
}