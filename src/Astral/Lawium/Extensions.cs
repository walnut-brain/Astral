using LanguageExt;

namespace Astral.Lawium
{
    public static class Extensions
    {
        public static Option<T> TryGet<T>(this LawBook book)
            => book.TryGet(typeof(T)).Unbox<T>();

        public static T Get<T>(this LawBook book)
            => book.TryGet<T>().Unwrap($"Value for type {typeof(T)} not found");

        
        
        
    }
}