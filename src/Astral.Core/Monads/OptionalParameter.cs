using System;

namespace Astral
{
    public struct OptionalParameter<T>
    {
        private readonly bool _specified;

        private readonly T _value;

        public OptionalParameter(T value) : this()
        {
            _value = value;
            _specified = true;
        }
        
        public static implicit operator OptionalParameter<T>(T value)
                => new OptionalParameter<T>(value);

        public static implicit operator Option<T>(OptionalParameter<T> value)
            => value._specified ? value._value.ToOption() : Option.None;

        public T WhenSkipped(T value)
            => _specified ? _value : value;

        public T WhenSkipped(Func<T> factory)
            => _specified ? _value : factory();
    }
}