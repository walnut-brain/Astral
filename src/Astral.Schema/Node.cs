using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace Astral.Schema
{
    public abstract class Node
    {
        private static long _nextId;
        public static long NewId() => Interlocked.Increment(ref _nextId);
        
        public long Id { get; }

        protected Node()
        {
            Id = NewId();
        }

        protected Node(Node @base)
        {
            if (@base == null) throw new ArgumentNullException(nameof(@base));
            Id = @base.Id;
            
        }
        
        protected abstract IEnumerable<Node> SubNodes { get; }

        protected Node FindNode(Func<Node, bool> predicate) 
            => predicate(this) ? this : SubNodes.Select(chield => chield.FindNode(predicate)).FirstOrDefault();

        protected IEnumerable<Node> FindNodes(Func<Node, bool> predicate)
        {
            if (predicate(this)) yield return this;
            foreach (var chield in SubNodes.SelectMany(p => p.FindNodes(predicate)))
            {
                yield return chield;
            }
        }

        protected abstract Node ReplaceNode(Node oldNode, Node newNode);

        protected bool Equals(Node other)
        {
            return Id == other.Id && Equals(SubNodes, other.SubNodes);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Node) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Id.GetHashCode() * 397) ^ SubNodes.GetHashCode();
            }
        }
    }

    public abstract class Leaf : Node
    {
        protected override IEnumerable<Node> SubNodes = Enumerable.Empty<Node>();
        

        protected override Node ReplaceNode(Node oldNode, Node newNode)
            => this;
    }
    
}