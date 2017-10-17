using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Astral.Schema
{
    internal class SimpleValueGreenElement<T> : SimpleValueGreenElement
    {
        internal SimpleValueGreenElement(long id, SimpleValueType type, T value) : base(id, type)
        {
            
            Value = value;
        }

        internal SimpleValueGreenElement(SimpleValueType type, T value) : base(type)
        {
            Value = value;
        }

        public T Value { get; }
        public override object ObjectValue => Value;

        protected bool Equals(SimpleValueGreenElement<T> other)
        {
            return base.Equals(other) && EqualityComparer<T>.Default.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SimpleValueGreenElement<T>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ (ReferenceEquals(Value, null) ? 0 : EqualityComparer<T>.Default.GetHashCode(Value));
            }
        }
    }

    internal abstract class SimpleValueGreenElement : ValueGreenElement
    {
        protected SimpleValueGreenElement(long id, SimpleValueType type) : base(id)
        {
            Type = type;
        }

        protected SimpleValueGreenElement(SimpleValueType type)
        {
            Type = type;
        }

        public SimpleValueType Type { get; }
        public abstract object ObjectValue { get; }

        protected override IReadOnlyCollection<SchemaGreenElement> Children { get; } =
            ImmutableList<SchemaGreenElement>.Empty;
        

        private static SimpleValueGreenElement<T> MakeNew<T>(T value)
            => new SimpleValueGreenElement<T>(FromType(typeof(T)), value);

        public static SimpleValueGreenElement<byte> Create(byte value)
            => MakeNew(value);
        
        public static SimpleValueGreenElement<sbyte> Create(sbyte value)
            => MakeNew(value);
        
        public static SimpleValueGreenElement<ushort> Create(ushort value)
            => MakeNew(value);
        
        public static SimpleValueGreenElement<short> Create(short value)
            => MakeNew(value);
        
        public static SimpleValueGreenElement<uint> Create(uint value)
            => MakeNew(value);
        
        public static SimpleValueGreenElement<int> Create(int value)
            => MakeNew(value);
        
        public static SimpleValueGreenElement<ulong> Create(ulong value)
            => MakeNew(value);
        
        public static SimpleValueGreenElement<long> Create(long value)
            => MakeNew(value);
        
        public static SimpleValueGreenElement<float> Create(float value)
            => MakeNew(value);
        
        public static SimpleValueGreenElement<double> Create(double value)
            => MakeNew(value);
        
        public static SimpleValueGreenElement<DateTime> Create(DateTime value)
            => MakeNew(value);
        
        public static SimpleValueGreenElement<DateTimeOffset> Create(DateTimeOffset value)
            => MakeNew(value);
        
        public static SimpleValueGreenElement<Guid> Create(Guid value)
            => MakeNew(value);
        
        public static SimpleValueGreenElement<TimeSpan> Create(TimeSpan value)
            => MakeNew(value);
        
        public static SimpleValueGreenElement<string> Create(string value)
            => MakeNew(value);

        private SimpleValueGreenElement<T> SetValue<T>(T value)
            => this is SimpleValueGreenElement<T> v
                ? Equals(v.Value, value) ? v
                : new SimpleValueGreenElement<T>(Id, FromType(typeof(T)), value)
                : new SimpleValueGreenElement<T>(Id, FromType(typeof(T)), value);
                
        
        public SimpleValueGreenElement<byte> WithValue(byte value)
            => SetValue(value);
        
        public SimpleValueGreenElement<sbyte> WithValue(sbyte value)
            => SetValue(value);
        
        public SimpleValueGreenElement<ushort> WithValue(ushort value)
            => SetValue(value);
        
        public SimpleValueGreenElement<short> WithValue(short value)
            => SetValue(value);
        
        public SimpleValueGreenElement<uint> WithValue(uint value)
            => SetValue(value);
        
        public SimpleValueGreenElement<int> WithValue(int value)
            => SetValue(value);
        
        public SimpleValueGreenElement<ulong> WithValue(ulong value)
            => SetValue(value);
        
        public SimpleValueGreenElement<long> WithValue(long value)
            => SetValue(value);
        
        public SimpleValueGreenElement<float> WithValue(float value)
            => SetValue(value);
        
        public SimpleValueGreenElement<double> WithValue(double value)
            => SetValue(value);
        
        public SimpleValueGreenElement<DateTime> WithValue(DateTime value)
            => SetValue(value);
        
        public SimpleValueGreenElement<DateTimeOffset> WithValue(DateTimeOffset value)
            => SetValue(value);
        
        public SimpleValueGreenElement<Guid> WithValue(Guid value)
            => SetValue(value);
        
        public SimpleValueGreenElement<TimeSpan> WithValue(TimeSpan value)
            => SetValue(value);
        
        public SimpleValueGreenElement<string> WithValue(string value)
            => SetValue(value);


        private static SimpleValueType FromType(Type type)
        {
            if (type == typeof(byte)) return SimpleValueType.U8;
            if (type == typeof(sbyte)) return SimpleValueType.I8;
            if (type == typeof(ushort)) return SimpleValueType.U16;
            if (type == typeof(short)) return SimpleValueType.I16;
            if (type == typeof(uint)) return SimpleValueType.U32;
            if (type == typeof(int)) return SimpleValueType.I32;
            if (type == typeof(ulong)) return SimpleValueType.U64;
            if (type == typeof(long)) return SimpleValueType.I64;
            if (type == typeof(float)) return SimpleValueType.F32;
            if (type == typeof(double)) return SimpleValueType.F64;
            if (type == typeof(DateTime)) return SimpleValueType.DT;
            if (type == typeof(DateTimeOffset)) return SimpleValueType.DTO;
            if (type == typeof(TimeSpan)) return SimpleValueType.TimeSpan;
            if (type == typeof(Guid)) return SimpleValueType.Uuid;
            if (type == typeof(string)) return SimpleValueType.String;
            throw new ArgumentOutOfRangeException($"Unknown simple value type for {type}");
        }

        protected bool Equals(SimpleValueGreenElement other)
        {
            return base.Equals(other) && Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((SimpleValueGreenElement) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ (int) Type;
            }
        }
    }
}