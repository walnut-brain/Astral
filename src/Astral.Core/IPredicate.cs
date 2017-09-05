namespace Astral
{
    public interface IPredicate<in T>
    {
        (bool, string) True(T value);
    }
}