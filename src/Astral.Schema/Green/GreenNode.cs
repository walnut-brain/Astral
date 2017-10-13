using System.Threading;

namespace Astral.Schema.Green
{
    public class GreenNode
    {
        private static int _nextId;

        public int Id { get; }
        public GreenNode()
        {
            Id = Interlocked.Increment(ref _nextId);
        }

        public GreenNode(GreenNode @base)
        {
            Id = @base.Id;
        }
    }
}