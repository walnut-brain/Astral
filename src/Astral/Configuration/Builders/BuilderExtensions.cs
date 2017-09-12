using System;
using System.Collections.Generic;
using Astral.Configuration.Settings;
using Astral.Deliveries;
using Astral.Payloads.Serialization;
using FunEx.Monads;
using Lawium;
using Newtonsoft.Json;
using Polly;

namespace Astral.Configuration.Builders
{
    public static class BuilderExtensions
    {
        
        
        public static TBuilder DeliveryOnSuccess<TBuilder>(this TBuilder builder, DeliveryOnSuccess onSuccess)
            where TBuilder : BuilderBase
        {
            builder.AddLaw(Law<Fact>.Axiom(new DeliveryOnSuccessSetting(onSuccess)));
            return builder;
            
        }
        

        public static TBuilder MessageTtl<TBuilder>(this TBuilder builder, TimeSpan ttl)
            where TBuilder : BuilderBase
        {
            builder.AddLaw(Law<Fact>.Axiom(new MessageTtlSetting(ttl)));
            return builder;
        }
        
        public static TBuilder MessageTtl<TBuilder, TMessage>(this TBuilder builder, Func<TMessage, TimeSpan> messageTtl)
            where TBuilder : BuilderBase
        {
            builder.AddLaw(Law<Fact>.Axiom(new MessageTtlFactorySetting<TMessage>(messageTtl)));
            return builder;
        }

        public static TBuilder DeliveryReplayTo<TBuilder>(this TBuilder builder, DeliveryReplyTo replayTo)
            where TBuilder : BuilderBase
        {
            builder.AddLaw(Law<Fact>.Axiom(new DeliveryReplayToSetting(replayTo)));
            return builder;
        }
        
        public static BusBuilder UseJsonSerializer<TBuilder>(this BusBuilder builder, JsonSerializerSettings jsettings = null)
            where TBuilder : BuilderBase
        {
            jsettings = jsettings ?? new JsonSerializerSettings();
            builder.SetSerializer(new Serialization<byte[]>(Serialization.JsonRawSerializeProvider(jsettings),
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