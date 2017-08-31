namespace Astral.Markup
{
    public interface IPredicate<in T>
    {
        (bool, string) True(T value);
    }
}