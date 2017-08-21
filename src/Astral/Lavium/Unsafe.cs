using System;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace Astral.Lavium
{
    public static class Unsafe
    {
        public static void RegisterAxiom(LawBook book, Type type, object value, bool externallyOwned)
        {
            book.RegisterAxiom(type, value, externallyOwned);
        }

        public static Option<object> GetFact(LawBook book, Type type)
            => book.GetFact(type);

        public static object GetExternalFact(LawBook book, Type key)
            => book.GetExternalFact(key);

        public static void SetExternalFact(LawBook book, Type key, object value)
            => book.SetExternalFact(key, value);
        
        public static Law CreateLaw(string name, Arr<Type> arguments, Arr<Type> findings,
            Func<ILogger, Func<Type, object>, Arr<object>, Arr<object>> executor)
            => new Law(name, arguments, findings, executor);
    }
}