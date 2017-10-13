using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Astral.Schema.Green
{
    public class WellKnownTypeSchemaGreen : TypeSchemaGreen, IHasSchemaName
    {
        private WellKnownTypeSchemaGreen(string name, Type dotNetType) : base(dotNetType, true)
        {
            if (dotNetType == null) throw new ArgumentNullException(nameof(dotNetType));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            SchemaName = name;
        }

        public string SchemaName { get; }


        
        

        static WellKnownTypeSchemaGreen()
        {
            var lst = new[]
            {
                new WellKnownTypeSchemaGreen("u8", typeof(byte)),
                new WellKnownTypeSchemaGreen("i8", typeof(sbyte)),
                new WellKnownTypeSchemaGreen("u16", typeof(ushort)),
                new WellKnownTypeSchemaGreen("i16", typeof(short)),
                new WellKnownTypeSchemaGreen("u32", typeof(uint)),
                new WellKnownTypeSchemaGreen("i32", typeof(int)),
                new WellKnownTypeSchemaGreen("u64", typeof(ulong)),
                new WellKnownTypeSchemaGreen("i64", typeof(long)),
                new WellKnownTypeSchemaGreen("f32", typeof(float)),
                new WellKnownTypeSchemaGreen("f64", typeof(double)),
                new WellKnownTypeSchemaGreen("string", typeof(string)),
                new WellKnownTypeSchemaGreen("uuid", typeof(Guid)),      
                new WellKnownTypeSchemaGreen("datetime", typeof(DateTime)),
                new WellKnownTypeSchemaGreen("datetime2", typeof(DateTimeOffset)),
                new WellKnownTypeSchemaGreen("timespan", typeof(TimeSpan))
            };
            
            ByCode = ImmutableDictionary.CreateRange(lst.Select(p => new KeyValuePair<string, WellKnownTypeSchemaGreen>(p.SchemaName, p)));
            var byTypeBuilder = ImmutableDictionary.CreateBuilder<Type, WellKnownTypeSchemaGreen>();
            byTypeBuilder.AddRange(lst.Select(p => new KeyValuePair<Type, WellKnownTypeSchemaGreen>(p.DotNetType.Unwrap(), p)));
            ByType = byTypeBuilder.ToImmutable();
        }

        public static ImmutableDictionary<string, WellKnownTypeSchemaGreen> ByCode { get; }
        public static ImmutableDictionary<Type, WellKnownTypeSchemaGreen> ByType { get; }

    }
}