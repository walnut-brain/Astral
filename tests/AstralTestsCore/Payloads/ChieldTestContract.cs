using Astral;

namespace AstralTests.Payloads
{
    [Contract("chield.test")]
    public class ChieldTestContract : TestContract
    {
        public int A { get; set; }
    }
}