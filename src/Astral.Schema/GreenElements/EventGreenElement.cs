using System;
using System.Diagnostics.CodeAnalysis;

namespace Astral.Schema
{
    internal class EventGreenElement : EndpointGreenElement
    {
        private EventGreenElement(long id, string name, string codeNameHint, string contentType,
            ExtensionCollectionGreenElement extensions, TypeOrTypeReferenceElement eventType) : base(id, name,
            codeNameHint, contentType, extensions)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            EventType = eventType ?? throw new ArgumentNullException(nameof(eventType));
        }

        public EventGreenElement(string name, string codeNameHint, string contentType,
            ExtensionCollectionGreenElement extensions, TypeOrTypeReferenceElement eventType) : base(name, codeNameHint,
            contentType, extensions)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            EventType = eventType ?? throw new ArgumentNullException(nameof(eventType));
        }

        public TypeOrTypeReferenceElement EventType { get; }

        [SuppressMessage("ReSharper", "ParameterHidesMember")]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public EventGreenElement With(OptionalParameter<string> Name, OptionalParameter<string> CodeNameHint,
            OptionalParameter<string> ContentType, OptionalParameter<ExtensionCollectionGreenElement> Extensions,
            OptionalParameter<TypeOrTypeReferenceElement> EventType)
        {
            var name = Name.WhenSkipped(this.Name);
            var codeNameHint = CodeNameHint.WhenSkipped(this.CodeNameHint);
            var contentType = ContentType.WhenSkipped(this.ContentType);
            var extensions = Extensions.WhenSkipped(this.Extensions) ?? new ExtensionCollectionGreenElement(null);
            var eventType = EventType.WhenSkipped(this.EventType);
            if(name != this.Name || codeNameHint != this.CodeNameHint ||
               contentType != this.ContentType || !Equals(extensions, this.Extensions) || !Equals(eventType, this.EventType) )
                return new EventGreenElement(Id, name, codeNameHint, contentType, extensions, eventType);
            return this;
        }

        protected bool Equals(EventGreenElement other)
        {
            return base.Equals(other) && EventType.Equals(other.EventType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((EventGreenElement) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ EventType.GetHashCode();
            }
        }
    }
}