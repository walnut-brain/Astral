using System;

namespace Astral.Delivery
{
    public abstract class ReleaseAction
    {
        private ReleaseAction()
        {
        }

        internal class DeleteType : ReleaseAction
        {
            protected bool Equals(DeleteType other) => true;
            

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((DeleteType) obj);
            }

            public override int GetHashCode() => typeof(DeleteType).GetHashCode();

        }

        internal class ArchiveType : ReleaseAction
        {
            public ArchiveType(DateTimeOffset deleteAt)
            {
                DeleteAt = deleteAt;
            }

            public DateTimeOffset DeleteAt { get; }

            protected bool Equals(ArchiveType other)
            {
                return DeleteAt.Equals(other.DeleteAt);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((ArchiveType) obj);
            }

            public override int GetHashCode()
            {
                return DeleteAt.GetHashCode();
            }
        }

        internal class RedeliveryType : ReleaseAction
        {
            public RedeliveryType(DateTimeOffset redeliveryAt)
            {
                RedeliveryAt = redeliveryAt;
            }

            public DateTimeOffset RedeliveryAt { get; }

            protected bool Equals(RedeliveryType other)
            {
                return RedeliveryAt.Equals(other.RedeliveryAt);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((RedeliveryType) obj);
            }

            public override int GetHashCode()
            {
                return RedeliveryAt.GetHashCode();
            }
        }

        public class ErrorType : ReleaseAction
        {
            public ErrorType(Exception exception)
            {
                Exception = exception;
            }

            public Exception Exception { get; }

            protected bool Equals(ErrorType other)
            {
                return Equals(Exception, other.Exception);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((ErrorType) obj);
            }

            public override int GetHashCode()
            {
                return (Exception != null ? Exception.GetHashCode() : 0);
            }
        }

        public class CancelType : ReleaseAction
        {
            protected bool Equals(CancelType other) => true;
            

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((CancelType) obj);
            }

            public override int GetHashCode() => typeof(CancelType).GetHashCode();
        }

        public static ReleaseAction Delete = new DeleteType();
        public static ReleaseAction Archive(DateTimeOffset deleteAt) => new ArchiveType(deleteAt);
        public static ReleaseAction Archive(TimeSpan after) => new ArchiveType(DateTimeOffset.Now + after);
        public static ReleaseAction Redelivery(DateTimeOffset redeliveryAt) => new RedeliveryType(redeliveryAt);
        public static ReleaseAction Redelivery(TimeSpan after) => new RedeliveryType(DateTimeOffset.Now + after);
        public static ReleaseAction Error(Exception ex) => new ErrorType(ex);
        public static ReleaseAction Cancel = new CancelType();
    }
}