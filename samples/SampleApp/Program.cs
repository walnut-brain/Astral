using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Astral;
using Astral.Configuration.Builders;
using Astral.Configuration.Configs;
using Astral.DependencyInjection;
using Astral.Serialization;
using Astral.Transport;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using SampleService;
using Serilog;
using Serilog.Formatting.Json;

namespace SampleApp
{
    public class FakeTranport : IMqTransport
    {
        public FakeTranport()
        {
        }

        public Func<Task> PreparePublish<T>(EndpointConfig config, T message, Serialized<byte[]> serialized, PublishOptions options)
        {
            throw new NotImplementedException();
        }

        public IDisposable Subscribe(EndpointConfig config, Func<Serialized<byte[]>, EventContext, CancellationToken, Task<Acknowledge>> handler, EventListenOptions options)
        {
            throw new NotImplementedException();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .WriteTo.LiterateConsole()
                .CreateLogger();

            var services = new ServiceCollection()
                .AddLogging()
                .AddSingleton<FakeTranport>();
            services.AddAstral();
            var builder = new BusBuilder("service");
            builder
                .UseDefault()
                .Service<ISampleService>().Endpoint(p => p.AwesomeEvent).MessageTtl(TimeSpan.FromSeconds(60));

            services.AddBus<FakeTranport>(builder);
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
            using (var container = containerBuilder.Build())
            {
                var bus = container.Resolve<Bus<FakeTranport>>();
                bus.Service<ISampleService>()
                    .PublishAsync(p => p.AwesomeEvent, new SampleEvent {Name = "Hellow", Id = 123}).Wait();
            }
        }
    }
}
