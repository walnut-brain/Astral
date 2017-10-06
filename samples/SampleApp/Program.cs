﻿using System;
using System.Reflection;
using System.Threading;
 using Astral.Links;
 using RabbitLink;
using RabbitLink.Messaging;
using RabbitLink.Services.Astral;
using SampleServices;

namespace SampleApp
{
    
    
    class Program
    {
        static void Main(string[] args)
        {

            using (var link =
                LinkBuilder
                    .Configure
                    .UseAstral("test")
                    .Uri("amqp://youdo:youdo@localhost")
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
                Thread.Sleep(1000);
                link.Service<IFirstService>().Event(p => p.Event)
                    .PublishAsync(new EventContract {Name = "test "});
                Console.ReadKey();
            }
        }
    }
}