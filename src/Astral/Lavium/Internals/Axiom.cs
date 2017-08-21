using System;

namespace Astral.Lavium.Internals
{
    internal class Axiom : IEquatable<Axiom>
    {
        public Axiom(Type id, object value, bool externallyOwned)
        {
            Id = id;
            Value = value;
            ExternallyOwned = externallyOwned;
        }

        public Type Id { get; }
        public object Value { get; }
        public bool ExternallyOwned { get; }

        public bool Equals(Axiom other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && Equals(Value, other.Value) && ExternallyOwned == other.ExternallyOwned;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Axiom) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Value != null ? Value.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ ExternallyOwned.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(Axiom left, Axiom right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Axiom left, Axiom right)
        {
            return !Equals(left, right);
        }
    }
}