namespace Astral.Fun.TypeClasses
{
    public interface SMonoid<T>
    {
        T Append(T value1, T value2);
        T Empty();
    }
}