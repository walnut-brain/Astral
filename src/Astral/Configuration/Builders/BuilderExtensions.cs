using System;
using System.Collections.Generic;
using System.Reflection;
using Astral.Configuration.Settings;
using Astral.Core;
using Astral.DataContracts;
using Astral.Delivery;
using Astral.Serialization;
using Astral.Serialization.Json;
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
            builder.AddLaw(Law.Axiom(new CleanSameKeyDelivery(clean)));
            return builder;
        }

        

        public static TBuilder DeliveryExceptionPolicy<TBuilder>(this TBuilder builder, Policy policy)
            where TBuilder : BuilderBase
        {
            builder.AddLaw(Law.Axiom<DeliveryExceptionPolicy>(new DeliveryExceptionPolicy(policy)));
            return builder;
        }

        public static TBuilder DeliveryReserveTime<TBuilder>(this TBuilder builder, TimeSpan reserve)
            where TBuilder : BuilderBase
        {
            builder.AddLaw(Law.Axiom(new DeliveryReserveTime(reserve)));
            return builder;
        }

        public static TBuilder AfterDelivery<TBuilder>(this TBuilder builder, ReleaseAction action)
            where TBuilder : BuilderBase
        {
            builder.AddLaw(Law.Axiom(new AfterDelivery(action)));
            return builder;
        }

        public static TBuilder IgnoreContractName<TBuilder>(this TBuilder builder, bool ignore = false)
            where TBuilder : BuilderBase
        {
            builder.AddLaw(Law.Axiom(new IgnoreContractName(ignore)));
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

        public static TBuilder MemberNameConverter<TBuilder>(this TBuilder builder,
            Func<string, bool, string> converter)
            where TBuilder : BuilderBase
        {
            builder.AddLaw(Law.Axiom(new MemberNameToAstralName(converter)));
            return builder;
        }

        public static TBuilder MessageTtl<TBuilder>(this TBuilder builder, TimeSpan ttl)
            where TBuilder : BuilderBase
        {
            builder.AddLaw(Law.Axiom(new MessageTtl(ttl)));
            return builder;
        }



        private class MessageKeyExtract<T> : IMessageKeyExtractor<T>
        {
            private readonly Func<T, string> _extractor;

            public MessageKeyExtract(Func<T, string> extractor)
            {
                _extractor = extractor;
            }

            public string ExtractKey(T message) => _extractor(message);

        }


        public static TBuilder UseDefaultTypeMapper<TBuilder>(this TBuilder builder, bool convertNames = false)
            where TBuilder : BuilderBase
        {
            var mapper = new DefaultTypeMapper();
            builder.AddLaw(Law.Axiom<ITypeToContractName>(mapper));
            builder.AddLaw(Law.Axiom<IContractNameToType>(mapper));
            if (convertNames)
                builder.AddLaw(Law.Create("TypeMapper with type name converter",
                    (MemberNameToAstralName cvt) =>
                    {
                        var map = new DefaultTypeMapper(p => cvt.Value(p, false));
                        return ((ITypeToContractName) map, (IContractNameToType) map);
                    }));


            return builder;
        }

        public static TBuilder UseJson<TBuilder>(this TBuilder builder, JsonSerializerSettings jsettings = null,
            bool checkContentType = false)
            where TBuilder : BuilderBase
        {
            jsettings = jsettings ?? new JsonSerializerSettings();
            builder.AddLaw(Law.Axiom(jsettings));
            builder.AddLaw(Law.Create("JsonSerializer", (JsonSerializerSettings settings) =>
                (
                (ISerialize<string>) new JsonTextSerialize(settings),
                (IDeserialize<string>) new JsonTextDeserialize(settings, checkContentType),
                (ISerializedMapper<string, byte[]>) new Utf8Mapper(),
                (ISerializedMapper<byte[], string>) new Utf8BackMapper(),
                UseSerializeMapper.Always
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


        public static TBuilder UseAttributes<TBuilder>(this TBuilder builder)
            where TBuilder : BuilderBase
        {
            return builder.AddLaws(new[]
            {
                Law.Create("ServiceNameFromServiceTypeName", (ServiceType st, MemberNameToAstralName cvt) =>
                    cvt.Value(st.Value.Name, true)),
                Law.Create("ServiceNameFromAttribute", (ServiceType mt) =>
                {
                    var attr = mt.Value.GetTypeInfo().GetCustomAttribute<ServiceAttribute>();
                    var name = attr?.Name;
                    var version = attr?.Version;
                    return (name == null ? null : new ServiceName(name),
                        version == null ? null : new ServiceVersion(version));
                }),
                Law.Create("EndpointNameFromMemberName", (PropertyInfo pi, MemberNameToAstralName cvt) =>
                    cvt.Value(pi.Name, false)),
                Law.Create("EndpointNameFromAttribute", (PropertyInfo mt) =>
                {
                    var name = mt.GetCustomAttribute<EndpointAttribute>()?.Name;
                    return name == null ? null : new EndpointName(name);
                })
            });
        }


        public static BusBuilder UseDefault(this BusBuilder builder)
        {
            return
                builder
                    .UseDefaultTypeMapper()
                    .UseJson()
                    .UseAttributes();
        }
    }
}