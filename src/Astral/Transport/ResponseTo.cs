namespace Astral.Transport
{
    public abstract class ResponseTo
    {
        private ResponseTo()
        {
        }

        public class SystemClass : ResponseTo
        {
            internal SystemClass()
            {
            }

            protected bool Equals(SystemClass other) => true;
            

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((SystemClass) obj);
            }

            public override int GetHashCode() => typeof(SystemClass).GetHashCode();

        }

        public class NamedClass : ResponseTo
        {
            internal NamedClass(string name)
            {
                Name = name;
            }

            public string Name { get; }

            protected bool Equals(NamedClass other)
            {
                return string.Equals(Name, other.Name);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((NamedClass) obj);
            }

            public override int GetHashCode()
            {
                return (Name != null ? Name.GetHashCode() : 0);
            }
        }

        public class InstanceClass : ResponseTo
        {
            internal InstanceClass()
            {
                
            }

            protected bool Equals(InstanceClass other) => true;
            

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((InstanceClass) obj);
            }

            public override int GetHashCode() => typeof(InstanceClass).GetHashCode();
        }

        public class NoneClass : ResponseTo
        {
            internal NoneClass() {}

            protected bool Equals(NoneClass other) => true;
            

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((NoneClass) obj);
            }

            public override int GetHashCode() => typeof(NoneClass).GetHashCode();
            
        }

        public class AnswerClass : ResponseTo
        {
            public string ReplyTo { get; }
            public string ReplyOn { get; }

            internal AnswerClass(string replyTo, string replyOn)
            {
                ReplyTo = replyTo;
                ReplyOn = replyOn;
            }

            protected bool Equals(AnswerClass other)
            {
                return string.Equals(ReplyTo, other.ReplyTo) && string.Equals(ReplyOn, other.ReplyOn);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((AnswerClass) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((ReplyTo != null ? ReplyTo.GetHashCode() : 0) * 397) ^ (ReplyOn != null ? ReplyOn.GetHashCode() : 0);
                }
            }
        }
        
        public static readonly ResponseTo System = new SystemClass();
        public static ResponseTo Named(string name) => new NamedClass(name);
        public static readonly ResponseTo Instance = new InstanceClass();
        public static readonly ResponseTo None = new NoneClass();
        public static ResponseTo Answer(string replyTo, string replayOn) => new AnswerClass(replyTo, replayOn);
    }
}