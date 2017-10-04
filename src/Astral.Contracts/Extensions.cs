using Astral.Links;

namespace Astral
{
    public static class Extensions
    {
        public static void Ack<T>(this IAck<T> ack) => ack.SetResult(Acknowledge.Ack);
        public static void Nack<T>(this IAck<T> ack) => ack.SetResult(Acknowledge.Nack);
        public static void Requeue<T>(this IAck<T> ack) => ack.SetResult(Acknowledge.Requeue);
    }
}