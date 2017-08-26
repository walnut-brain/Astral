using Astral.Exceptions;

namespace Astral
{
    public class EventContext
    {
        public EventContext(string sender)
        {
            Sender = sender;
        }

        public string Sender { get; }

        public void Requeue(string reason = null)
        {
            if (reason == null)
                throw new RequeuException();
            throw new RequeuException(reason);
        }

        public void Nack(string reason = null)
        {
            if (reason == null)
                throw new NackException();
            throw new NackException(reason);
        }
    }
}