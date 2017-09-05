using System.Runtime.Serialization;
using Astral;
using AstralTests.Payloads.DataContracts;

namespace AstralTests.Payloads
{
    [Contract("test.contract")]
    [KnownType(typeof(ChieldTestContract))]
    public class TestContract
    {
        public string Name { get; set; }
    }
}