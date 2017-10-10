﻿﻿using System;
 using System.Text;
 using System.Threading;
 using System.Threading.Tasks;
 using Astral.Liaison;
 using Astral.RabbitLink;
 using Microsoft.Extensions.Logging;
 using RabbitLink.Messaging;
 using RabbitLink.Serialization.Json;
 using SampleServices;
 using Serilog;
 using Serilog.Formatting.Json;

namespace SampleApp
{
    
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

            var loggerFactory = new LoggerFactory();
            loggerFactory.AddSerilog();
            
 

            using (var link =
                new ServiceLinkBuilder()
                    .HolderName("test")
                    .Uri("amqp://youdo:youdo@localhost")
                    .AutoStart(true)
                    .ConnectionName("Test process")
                    .LoggerFactory(loggerFactory)
                    .Serializer(new LinkJsonSerializer())
                    .Build())
            {
                var service = link.Service<IFirstService>().Schema;
                link.Service<IFirstService>().Event(p => p.Event)
                    .Listen(async (p, ct) =>
                    {
                        Console.WriteLine(p.Name);
                        return Acknowledge.Ack;
                    });
                link.Service<IFirstService>().Call(p => p.Call)
                    .Process((p, c) => Task.FromResult(p + 5));
                Thread.Sleep(1000);
                
                
                /*
                link.Service<IFirstService>().Event(p => p.Event)
                    .PublishAsync(new EventContract {Name = "test "});*/
                link
                    .Producer
                    .Exchange(cfg => cfg.ExchangeDeclarePassive("test.first"))
                    .PublishProperties(new LinkPublishProperties
                    {
                        RoutingKey = "event"
                    })
                    .Build().PublishAsync(new LinkPublishMessage<EventContract>(new EventContract {Name = "empty"}));
                link.Service<IFirstService>().Call(p => p.Call)
                    .ResponseQueueExpires(TimeSpan.FromSeconds(5))
                    .Call(10, CancellationToken.None).ContinueWith(p => Console.WriteLine(p.Result));
                Console.ReadKey();
            }
        }
    }
}