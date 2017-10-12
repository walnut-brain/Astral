using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Astral.Schema.Green
{
    public class WellKnownTypeDescriptionSchemaGreen : TypeDescriptionSchemaGreen
    {
        private WellKnownTypeDescriptionSchemaGreen(string name, Type dotNetType) : base(NextId, name, dotNetType, true)
        {
        }

        static WellKnownTypeDescriptionSchemaGreen()
        {
            var lst = new[]
            {
                new WellKnownTypeDescriptionSchemaGreen("u8", typeof(byte)),
                new WellKnownTypeDescriptionSchemaGreen("i8", typeof(byte)),
                new WellKnownTypeDescriptionSchemaGreen("u16", typeof(byte)),
                new WellKnownTypeDescriptionSchemaGreen("i16", typeof(byte)),
                new WellKnownTypeDescriptionSchemaGreen("u32", typeof(byte)),
                new WellKnownTypeDescriptionSchemaGreen("i32", typeof(byte)),
                new WellKnownTypeDescriptionSchemaGreen("u64", typeof(byte)),
                new WellKnownTypeDescriptionSchemaGreen("i64", typeof(byte)),
                new WellKnownTypeDescriptionSchemaGreen("f32", typeof(byte)),
                new WellKnownTypeDescriptionSchemaGreen("f64", typeof(byte)),
                new WellKnownTypeDescriptionSchemaGreen("string", typeof(byte)),
                new WellKnownTypeDescriptionSchemaGreen("uuid", typeof(byte)),      
                new WellKnownTypeDescriptionSchemaGreen("date", typeof(byte)),
                new WellKnownTypeDescriptionSchemaGreen("timespan", typeof(byte))
            };
            
            var byCodeBuilder = ImmutableDictionary.CreateBuilder<string, WellKnownTypeDescriptionSchemaGreen>();
            byCodeBuilder.AddRange(lst.Select(p => new KeyValuePair<string, WellKnownTypeDescriptionSchemaGreen>(p.Name, p)));
            ByCode = byCodeBuilder.ToImmutable();
            var byTypeBuilder = ImmutableDictionary.CreateBuilder<Type, WellKnownTypeDescriptionSchemaGreen>();
            byTypeBuilder.AddRange(lst.Select(p => new KeyValuePair<Type, WellKnownTypeDescriptionSchemaGreen>(p.DotNetType, p)));
            ByType = byTypeBuilder.ToImmutable();
        }

        public static ImmutableDictionary<string, WellKnownTypeDescriptionSchemaGreen> ByCode { get; }
        public static ImmutableDictionary<Type, WellKnownTypeDescriptionSchemaGreen> ByType { get; }

    }
}