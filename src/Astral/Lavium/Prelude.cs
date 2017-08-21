using System;
using System.Threading;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace Astral.Lavium
{
    public static partial class Prelude
    {
        
        
        public static T WithEffect<T>(this T value, Action action)
        {
            action();
            return value;
        }
        
        public static T WithEffect<T>(this T value, Action<T> action)
        {
            action(value);
            return value;
        }


        private static int _nextLawId;

        internal static int NextLawId => Interlocked.Increment(ref _nextLawId);


        public static TResult Lock<T, TResult>(this T locker, Func<T, TResult> calc)
            where T : class 
        {
            lock (locker)
            {
                return calc(locker);
            }
        }

        public static TResult Lock<T, TResult>(this T locker, Func<TResult> calc)
            where T : class
            => Lock(locker, _ => calc());
        
        public static void Lock<T>(this T locker, Action<T> calc)
            where T : class
        {
            lock (locker) 
                calc(locker);
        }

        public static void Lock<T>(this T locker, Action calc)
            where T : class
            => Lock(locker, _ => calc());

        public static void RegisterAxiom<T>(this LawBook book, T value, bool externallyOwned = false)
            => book.RegisterAxiom(typeof(T), value, externallyOwned);

        public static T Get<T>(this LawBook book)
            => book.GetFact(typeof(T)).UnboxUnsafe<T>().Unwrap($"Type {typeof(T)} not found in book");

        
        
        public static Option<T> TryGet<T>(this LawBook book)
            => book.GetFact(typeof(T)).Unbox<T>();

        public static T GetExtenalFact<T>(this LawBook book)
            => (T) book.GetExternalFact(typeof(T));

        public static void SetExternalFact<T>(this LawBook book, T fact)
            => book.SetExternalFact(typeof(T), fact);


        
    }
}