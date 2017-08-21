namespace Astral.Lavium
{
    internal class LawRec
    {
        public LawRec(int order, bool processed, Law law)
        {
            Order = order;
            Processed = processed;
            Law = law;
        }

        public int Order { get; set; }
        public bool Processed { get; set; }
        public Law Law { get; set; }
    }
}