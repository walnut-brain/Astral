﻿using System;
 using System.Threading;
 using System.Threading.Tasks;
 using Astral.Liaison;
 using Astral.RabbitLink;
 using SampleServices;

namespace SampleApp
{
    
    class Program
    {
        
        
        
        static void Main(string[] args)
        {
            

            using (var link =
                new ServiceLinkBuilder()
                    .HolderName("test")
                    .Uri("amqp://localhost")
                    .AutoStart(true)
                    .ConnectionName("Test process")
                    .Build())
            {
                link.Service<IFirstService>().Event(p => p.Event)
                    .Listen(async (p, ct) =>
                    {
                        Console.WriteLine(p.Name);
                        return Acknowledge.Ack;
                    });
                link.Service<IFirstService>().Call(p => p.Call)
                    .Process((p, c) => Task.FromResult(p + 5));
                Thread.Sleep(1000);
                link.Service<IFirstService>().Event(p => p.Event)
                    .PublishAsync(new EventContract {Name = "test "});
                link.Service<IFirstService>().Call(p => p.Call)
                    .Call(10, CancellationToken.None).ContinueWith(p => Console.WriteLine(p.Result));
                Console.ReadKey();
            }
        }
    }
}