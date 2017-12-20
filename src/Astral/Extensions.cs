using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Reflection;
using System.Threading.Tasks;
using Astral.Logging;

namespace Astral
{
    public static partial class Extensions
    {
        public static PropertyInfo GetProperty<TOwner, TValue>(this Expression<Func<TOwner, TValue>> selector)
        {
            return TryGetProperty(selector) ?? throw new ArgumentException($"Invalid property selector {selector}");
        }

        private static PropertyInfo TryGetProperty<TOwner, TValue>(Expression<Func<TOwner, TValue>> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            var memberExpr = selector.Body as MemberExpression;
            var propInfo = memberExpr?.Member as PropertyInfo;
            return propInfo;
        }

        

        
        
        
        public static IEnumerable<T> AsEnumerable<T>(this T value) => new[] {value};
        
        public static void LogActivity(this ILog logger, Action action, string message)
        {
            try
            {
                logger.Trace("Starting - " + message);
                action();
                logger.Trace("Success - " + message);
            }
            catch (Exception ex)
            {
                logger.Error("Error - " + message, ex);
                throw;
            }
        }

        public static T LogActivity<T>(this ILog logger, Func<T> func, string message)
        {
            try
            {
                logger.Trace("Starting - " + message);
                var result = func();
                logger.Trace("Success - " + message);
                return result;
            }
            catch (Exception ex)
            {
                logger.Error("Error - " + message, ex);
                throw;
            }
        }
        
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

        public static async Task<T> LogResult<T>(this Task<T> task, ILog logger, string message)
        {
            try
            {
                logger.Trace("Starting - " + message);
                var result = await task;
                logger.Trace("Success - " + message);
                return result;
            }
            catch (Exception ex) when (ex.IsCancellation())
            {
                logger.Trace("Cancelled - " + message);
                throw;
            }
            catch (Exception ex1)
            {
                logger.Error("Cancelled - " + message, ex1);
                throw;
            }
        }

        public static async Task LogResult(this Task task, ILog logger, string message)
        {
            try
            {
                logger.Trace("Starting - " + message);
                await task;
                logger.Trace("Success - " + message);
            }
            catch (Exception ex) when (ex.IsCancellation())
            {
                logger.Trace("Cancelled - " + message);
                throw;
            }
            catch (Exception ex1)
            {
                logger.Error("Cancelled - " + message, ex1);
                throw;
            }
        }
        
        public static bool IsJson(this ContentType contentType)
        {
            var types = new[] {"text/json", "application/json"};

            return types.Any(p =>
                string.Compare(contentType.MediaType, p, StringComparison.InvariantCultureIgnoreCase) == 0);
        }
    }
    
}