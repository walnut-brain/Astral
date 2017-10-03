using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Reflection;

namespace RabbitLink.Services
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

        public static bool IsJson(this ContentType contentType)
        {
            var types = new[] {"text/json", "application/json"};

            return types.Any(p =>
                string.Compare(contentType.MediaType, p, StringComparison.InvariantCultureIgnoreCase) == 0);
        }

        

    }
}