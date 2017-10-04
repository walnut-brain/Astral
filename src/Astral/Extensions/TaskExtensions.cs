using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Astral.Extensions
{
    public static class TaskExtensions
    {
        public static bool IsCancellation(this Exception ex)
        {
            switch (ex)
            {
                case OperationCanceledException _:
                    return true;
                case AggregateException ae when ae.Flatten().InnerExceptions.Any(p => p is OperationCanceledException):
                    return true;
                default:
                    return false;
            }
        }

        public static async Task<T> CorrectError<T>(this Task<T> task, Func<Exception, T> corrector)
        {
            try
            {
                return await task;
            }
            catch (Exception ex)
            {
                return corrector(ex);
            }
        }

        public static async Task<T> LogResult<T>(this Task<T> task, ILogger logger, string message,
            params object[] args)
        {
            try
            {
                logger.LogTrace("Starting - " + message, args);
                var result = await task;
                logger.LogTrace("Success - " + message, args);
                return result;
            }
            catch (Exception ex) when (ex.IsCancellation())
            {
                logger.LogTrace("Cancelled - " + message, args);
                throw;
            }
            catch (Exception ex1)
            {
                logger.LogError(0, ex1, "Cancelled - " + message, args);
                throw;
            }
        }

        public static async Task LogResult(this Task task, ILogger logger, string message, params object[] args)
        {
            try
            {
                logger.LogTrace("Starting - " + message, args);
                await task;
                logger.LogTrace("Success - " + message, args);
            }
            catch (Exception ex) when (ex.IsCancellation())
            {
                logger.LogTrace("Cancelled - " + message, args);
                throw;
            }
            catch (Exception ex1)
            {
                logger.LogError(0, ex1, "Cancelled - " + message, args);
                throw;
            }
        }
    }
}