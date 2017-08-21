using LanguageExt;

namespace Astral.Lavium.Internals
{
    internal class BookSnapshot
    {
        public BookSnapshot(int version, Arr<Law> laws, Arr<Axiom> facts)
        {
            Version = version;
            Laws = laws;
            Facts = facts;
        }

        public int Version { get; }
        public Arr<Law> Laws { get; }
        public Arr<Axiom> Facts { get; } 
    }
}