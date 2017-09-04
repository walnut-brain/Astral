using System;
using System.Collections.Generic;
using System.Reflection;
using Astral.Configuration.Settings;
using Astral.Deliveries;
using Astral.Payloads.DataContracts;
using Astral.Payloads.Serialization;
using Astral.Settings;
using Lawium;
using Newtonsoft.Json;
using Polly;
using Contract = Astral.Payloads.DataContracts.Contract;

namespace Astral.Configuration.Builders
{
    public static class BuilderExtensions
    {
        public static TBuilder CleanSameKeyDelivery<TBuilder>(this TBuilder builder, bool clean)
            where TBuilder : BuilderBase
        {
            builder.AddLaw(Law.Axiom(new CleanSameKeyDelivery(clean)));
            return builder;
        }

        

        public static TBuilder DeliveryExceptionPolicy<TBuilder>(this TBuilder builder, Policy policy)
            where TBuilder : BuilderBase
        {
            builder.AddLaw(Law.Axiom<DeliveryExceptionPolicy>(new DeliveryExceptionPolicy(policy)));
            return builder;
        }

        public static TBuilder AfterDelivery<TBuilder>(this TBuilder builder, OnDeliverySuccess action)
            where TBuilder : BuilderBase
        {
            builder.AddLaw(Law.Axiom(new AfterDelivery(action)));
            return builder;
        }

        public static BusBuilder MessageKeyExtractor<TEvent>(this BusBuilder builder, Func<TEvent, string> extractKey)
        {
            builder.AddLaw(Law.Axiom(new MessageKeyExtract<TEvent>(extractKey)));
            return builder;
        }

        public static ServiceBuilder MessageKeyExtractor<TEvent>(this ServiceBuilder builder, Func<TEvent, string> extractKey)
        {
            builder.AddLaw(Law.Axiom(new MessageKeyExtract<TEvent>(extractKey)));
            return builder;
        }

        public static EventEndpointBuilder<TEvent> MessageKeyExtractor<TEvent>(this EventEndpointBuilder<TEvent> builder, Func<TEvent, string> extractKey)
        {
            builder.AddLaw(Law.Axiom(new MessageKeyExtract<TEvent>(extractKey)));
            return builder;
        }


        public static TBuilder MessageTtl<TBuilder>(this TBuilder builder, TimeSpan ttl)
            where TBuilder : BuilderBase
        {
            builder.AddLaw(Law.Axiom(new MessageTtl(ttl)));
            return builder;
        }



        public static TBuilder UseDefaultTypeMapper<TBuilder>(this TBuilder builder)
            where TBuilder : BuilderBase
        {
            builder.AddLaw(Law.Axiom(Contract.DefaultContractMapper.Loopback()));
            builder.AddLaw(Law.Axiom(Contract.DefaultTypeMapper.Loopback()));
            return builder;
        }

        public static TBuilder UseJson<TBuilder>(this TBuilder builder, JsonSerializerSettings jsettings = null,
            bool checkContentType = false)
            where TBuilder : BuilderBase
        {
            jsettings = jsettings ?? new JsonSerializerSettings();
            builder.AddLaw(Law.Axiom(jsettings));
            builder.AddLaw(Law.Create("JsonSerializer", (JsonSerializerSettings settings) =>
                (   Serialization.JsonRawDeserializeProvider(settings),
                    Serialization.JsonRawSerializeProvider(settings)
                )
            ));
            return builder;
        }

        public static TBuilder AddLaws<TBuilder>(this TBuilder builder, IEnumerable<Law> laws)
            where TBuilder : BuilderBase
        {
            foreach (var law in laws)
                builder.AddLaw(law);
            return builder;
        }


        


        public static BusBuilder UseDefaults(this BusBuilder builder)
        {
            return
                builder
                    .UseDefaultTypeMapper()
                    .UseJson();
        }
    }
}