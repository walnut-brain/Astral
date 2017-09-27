namespace Lawium
{
    internal class Inference
    {
        public Inference(object value, int lawIndex)
        {
            Value = value;
            LawIndex = lawIndex;
        }

        public object Value { get; }
        public int LawIndex { get; }
    }
}