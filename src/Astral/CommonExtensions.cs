using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Astral
{
    public static class CommonExtensions
    {
        public static PropertyInfo GetProperty<TOwner, TValue>(this Expression<Func<TOwner, TValue>> selector)
            => TryGetProperty(selector) ?? throw new ArgumentException($"Invalid property selector {selector}");

        private static PropertyInfo TryGetProperty<TOwner, TValue>(Expression<Func<TOwner, TValue>> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            var memberExpr = selector.Body as MemberExpression;
            var propInfo = memberExpr?.Member as PropertyInfo;
            return propInfo;
        }
    }
}