using System;

namespace Astral.Lavium.Internals
{
    internal class Inference : Axiom
    {
        public Inference(Type id, object value, bool externallyOwned, int lawId) : base(id, value, externallyOwned)
        {
            LawId = lawId;
        }

        public int LawId { get; }
    }
}