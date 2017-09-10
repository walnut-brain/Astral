using System;
using Astral.DependencyInjection;
using Autofac;
using Autofac.Astral;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AstralTestsCore.DependencyInjection
{
    public class DependencyInjectionTest
    {
        public class Extender : ITaggedScopeExtender
        {
            public Extender()
            {
            }

            public void Extend(object tag, IServiceCollection collection)
            {
                collection.AddScoped(sp => sp.GetRequiredService<Version>().ToString());
            }
        }


        [Fact]
        public void MustTakeDependencyFromParentOnFactoryCreation()
        {
            var builder = new ContainerBuilder();
            var parent = new ServiceCollection();
            parent.AddScoped(ssp => new Version(5, 1));
            parent.AddTransient<ITaggedScopeExtender, Extender>();
            builder.AddAstral();
            builder.Populate(parent);
            var container = builder.Build();
            var sp = container.Resolve<IServiceProvider>();
            var sc = sp.CreateScope("456");
            var sp1 = sc.ServiceProvider;
            Assert.Equal("5.1", sp1.GetService<string>());
        }
    }
}