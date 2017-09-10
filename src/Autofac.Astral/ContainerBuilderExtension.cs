using Astral.DependencyInjection;

namespace Autofac.Astral
{
    public static class ContainerBuilderExtension
    {
        public static void AddAstral(this ContainerBuilder builder)
        {
            builder.RegisterType<TaggedScopeProvider>().As<ITaggedScopeProvider>();
        }
    }
}