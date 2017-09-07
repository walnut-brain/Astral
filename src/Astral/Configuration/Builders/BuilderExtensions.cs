using System;
using System.Collections.Generic;
using Astral.Configuration.Settings;
using Astral.Deliveries;
using Astral.Payloads.Serialization;
using Lawium;
using Newtonsoft.Json;
using Polly;

namespace Astral.Configuration.Builders
{
    public static class BuilderExtensions
    {
        public static TBuilder CleanSameKeyDelivery<TBuilder>(this TBuilder builder, bool clean)
            where TBuilder : BuilderBase
        {
            builder.AddLaw(Law<Fact>.Axiom(new CleanSameKeyDelivery(clean)));
            return builder;
        }

        

        public static TBuilder DeliveryExceptionPolicy<TBuilder>(this TBuilder builder, Policy policy)
            where TBuilder : BuilderBase
        {
            builder.AddLaw(Law<Fact>.Axiom(new DeliveryExceptionPolicy(policy)));
            return builder;
        }

        public static TBuilder AfterDelivery<TBuilder>(this TBuilder builder, AfterCommitDelivery action)
            where TBuilder : BuilderBase
        {
            builder.AddLaw(Law<Fact>.Axiom(new AfterCommitDelivery(action)));
            return builder;
        }

        public static BusBuilder MessageKeyExtractor<TEvent>(this BusBuilder builder, Func<TEvent, string> extractKey)
        {
            builder.AddLaw(Law<Fact>.Axiom(new MessageKeyExtractor<TEvent>(extractKey)));
            return builder;
        }

        public static ServiceBuilder MessageKeyExtractor<TEvent>(this ServiceBuilder builder, Func<TEvent, string> extractKey)
        {
            builder.AddLaw(Law<Fact>.Axiom(new MessageKeyExtractor<TEvent>(extractKey)));
            return builder;
        }

        public static EventEndpointBuilder<TEvent> MessageKeyExtractor<TEvent>(this EventEndpointBuilder<TEvent> builder, Func<TEvent, string> extractKey)
        {
            builder.AddLaw(Law<Fact>.Axiom(new MessageKeyExtractor<TEvent>(extractKey)));
            return builder;
        }


        public static TBuilder MessageTtl<TBuilder>(this TBuilder builder, TimeSpan ttl)
            where TBuilder : BuilderBase
        {
            builder.AddLaw(Law<Fact>.Axiom(new MessageTtl(ttl)));
            return builder;
        }


        public static BusBuilder UseJsonSerializer<TBuilder>(this BusBuilder builder, JsonSerializerSettings jsettings = null)
            where TBuilder : BuilderBase
        {
            jsettings = jsettings ?? new JsonSerializerSettings();
            builder.SetSerializer(new Serializer<byte[]>(Serialization.JsonRawSerializeProvider(jsettings),
                Serialization.JsonRawDeserializeProvider(jsettings)));
            return builder;
        }

        public static TBuilder AddLaws<TBuilder>(this TBuilder builder, IEnumerable<Law<Fact>> laws)
            where TBuilder : BuilderBase
        {
            foreach (var law in laws)
                builder.AddLaw(law);
            return builder;
        }

    }
}