using System;

namespace Astral.Schema.Data
{
    public sealed class SimpleTypeDesc : TypeDesc
    {
        public SimpleTypeDesc(SimpleTypeKind kind)
        {
            Kind = kind;
        }

        public SimpleTypeKind Kind { get; }


        public override Type DotNetType
        {
            get
            {
                switch (Kind)
                {
                    case SimpleTypeKind.Unit:
                        return typeof(ValueTuple);                        
                    case SimpleTypeKind.Bool:
                        return typeof(bool);
                    case SimpleTypeKind.Byte:
                        return typeof(byte);
                    case SimpleTypeKind.SByte:
                        return typeof(sbyte);
                    case SimpleTypeKind.UInt16:
                        return typeof(ushort);
                    case SimpleTypeKind.Int16:
                        return typeof(short);
                    case SimpleTypeKind.UInt:
                        return typeof(uint);
                    case SimpleTypeKind.Int:
                        return typeof(int);
                    case SimpleTypeKind.UInt64:
                        return typeof(ulong);
                    case SimpleTypeKind.Int64:
                        return typeof(long);
                    case SimpleTypeKind.Float:
                        return typeof(float);
                    case SimpleTypeKind.Double:
                        return typeof(double);
                    case SimpleTypeKind.String:
                        return typeof(string);
                    case SimpleTypeKind.Guid:
                        return typeof(Guid);
                    case SimpleTypeKind.DateTimeOffset:
                        return typeof(DateTimeOffset);
                    case SimpleTypeKind.TimeSpan:
                        return typeof(TimeSpan);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public override string Contract
        {
            get
            {
                switch (Kind)
                {
                    case SimpleTypeKind.Unit:
                        return "unit";
                    case SimpleTypeKind.Bool:
                        return "bool";
                    case SimpleTypeKind.Byte:
                        return "u8";
                    case SimpleTypeKind.SByte:
                        return "i8";
                    case SimpleTypeKind.UInt16:
                        return "u16";
                    case SimpleTypeKind.Int16:
                        return "i16";
                    case SimpleTypeKind.UInt:
                        return "u32";
                    case SimpleTypeKind.Int:
                        return "i32";
                    case SimpleTypeKind.UInt64:
                        return "u64";
                    case SimpleTypeKind.Int64:
                        return "i64";
                    case SimpleTypeKind.Float:
                        return "f32";
                    case SimpleTypeKind.Double:
                        return "f64";
                    case SimpleTypeKind.String:
                        return "string";
                    case SimpleTypeKind.Guid:
                        return "uuid";
                    case SimpleTypeKind.DateTimeOffset:
                        return "date";
                    case SimpleTypeKind.TimeSpan:
                        return "time";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public static Option<SimpleTypeDesc> FromType(Type type)
        {
            if(type ==typeof(ValueTuple))
                return new SimpleTypeDesc(SimpleTypeKind.Unit);
                                    
            if(type ==typeof(bool))
                return new SimpleTypeDesc(SimpleTypeKind.Bool);
            
            if(type ==typeof(byte))
                return new SimpleTypeDesc(SimpleTypeKind.Byte);
            
            if(type ==typeof(sbyte))
                return new SimpleTypeDesc(SimpleTypeKind.SByte);
            
            if(type ==typeof(ushort))
                return new SimpleTypeDesc(SimpleTypeKind.UInt16);
            
            if(type ==typeof(short))
                return new SimpleTypeDesc(SimpleTypeKind.Int16);
            
            if(type ==typeof(uint))
                return new SimpleTypeDesc(SimpleTypeKind.UInt);
            
            if(type ==typeof(int))
                return new SimpleTypeDesc(SimpleTypeKind.Int);
            
            if(type ==typeof(ulong))
                return new SimpleTypeDesc(SimpleTypeKind.UInt64);
            
            if(type ==typeof(long))
                return new SimpleTypeDesc(SimpleTypeKind.Int64);
            
            if(type ==typeof(float))
                return new SimpleTypeDesc(SimpleTypeKind.Float);
            
            if(type ==typeof(double))
                return new SimpleTypeDesc(SimpleTypeKind.Double);
            
            if(type ==typeof(string))
                return new SimpleTypeDesc(SimpleTypeKind.String);
            
            if(type ==typeof(Guid))
                return new SimpleTypeDesc(SimpleTypeKind.Guid);
            
            if(type == typeof(DateTimeOffset) || type == typeof(DateTime))
                return new SimpleTypeDesc(SimpleTypeKind.DateTimeOffset);

            if (type == typeof(TimeSpan))
                return new SimpleTypeDesc(SimpleTypeKind.TimeSpan);

            return Option.None;
        }
    }
}