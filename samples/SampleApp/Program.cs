using System;
using System.Text;
using System.Threading.Tasks;
using Astral;
using Astral.Configuration.Builders;
using Astral.DependencyInjection;
using Astral.Payloads;
using Astral.Specifications;
using Astral.Transport;
using Autofac;
using Autofac.Astral;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using SampleService;
using Serilog;

namespace SampleApp
{
    public class FakeTranport : ITransport
    {
        public FakeTranport()
        {
        }

        public PayloadSender<TMessage> PreparePublish<TMessage>(EndpointSpecification specification, PublishOptions options)
        {
            throw new NotImplementedException();
        }

        public IDisposable Subscribe(EndpointSpecification specification, RawMessageHandler handler, EventListenOptions options)
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
                .AddSingleton<FakeTranport>()
                .AddAstral()
                .AddBus("Test", cfg =>
                    {
                        cfg.AddTransport<FakeTranport>();
                        cfg.Service<ISampleService>().Endpoint(p => p.AwesomeEvent)
                            .MessageTtl(TimeSpan.FromSeconds(60));

                    });


            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
            containerBuilder.AddAstral();
            using (var container = containerBuilder.Build())
            {
                var bus = container.Resolve<Bus>();
                bus.Service<ISampleService>()
                    .PublishAsync(p => p.AwesomeEvent, new SampleEvent {Name = "Hellow", Id = 123}).Wait();
            }
        }
    }
}
