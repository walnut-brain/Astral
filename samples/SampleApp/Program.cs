using System;
using Astral.Configuration.Builders;
using SampleService;

namespace SampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new BusBuilder("service")
                .UseDefault()
                .Service<ISampleService>().Endpoint(p => p.AwesomeEvent).MessageTtl(TimeSpan.FromSeconds(60));
            
        }
    }
}
