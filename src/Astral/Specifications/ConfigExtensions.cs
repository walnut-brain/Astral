using Astral.Exceptions;
using FunEx;
using FunEx.Monads;

namespace Astral.Specifications
{
    public static class ConfigExtensions
    {
        public static Result<T> AsTry<T>(this SpecificationBase specification) where T : Fact
        {
            return specification.TryGetService<T>()
                .ToResult(new InvalidConfigurationException($"Cannot find config setting {typeof(T)}"));
        }

        public static bool TryGetService<T>(this SpecificationBase specification, out T result) where T : Fact
        {
            var data = default(T);
            var res = specification.TryGetService<T>().Match(p =>
            {
                data = p;
                return true;
            }, () => false);
            result = data;
            return res;
        }

        

        
    }
}