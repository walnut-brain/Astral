namespace Astral.Configuration.Configs
{
    public static class Extensions
    {
        public static T Get<T>(this ConfigBase config)
            => config.TryGet<T>().Unwrap($"Cannot find config setting {typeof(T)}");
    }
}