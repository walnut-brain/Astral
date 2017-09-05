using System.Net.Mime;
using Astral;
using Astral.Payloads;
using Astral.Payloads.DataContracts;
using Astral.Payloads.Serialization;
using Newtonsoft.Json;
using Xunit;
using System;

namespace AstralTests.Payloads
{
    public class PayloadTests
    {
        [Fact]
        public void ComplexDefaultTest()
        {
            var toPayOpt = new ToPayloadOptions<byte[]>(new ContentType("text/json; charset=utf-8"),
                Contract.DefaultTypeMapper(WellKnownTypes.Default).Loopback(),
                Serialization.JsonRawSerializeProvider(new JsonSerializerSettings()));
            var fromPayOpt = new FromPayloadOptions<byte[]>(Contract.DefaultContractMapper(WellKnownTypes.Default).Loopback(),
                Serialization.JsonRawDeserializeProvider(new JsonSerializerSettings()));
            var test = new TestContract[]
            {
                new ChieldTestContract { Name = "123", A = 5 },
                new ChieldTestContract { Name = "234", A = 6}, 
            };
            var payload = Payload.ToPayload(test, toPayOpt).Unwrap();
            var res = Payload.FromPayload(payload, fromPayOpt).As<TestContract[]>().Unwrap();
            
            Assert.Equal(test.GetType(), res.GetType());
            Assert.Equal(test[1].Name, res[1].Name);

        }
        
        [Fact]
        public void ValueTupleDefaultTest()
        {
            var toPayOpt = new ToPayloadOptions<byte[]>(new ContentType("text/json; charset=utf-8"),
                Contract.DefaultTypeMapper(WellKnownTypes.Default).Loopback(),
                Serialization.JsonRawSerializeProvider(new JsonSerializerSettings()));
            var fromPayOpt = new FromPayloadOptions<byte[]>(Contract.DefaultContractMapper(WellKnownTypes.Default).Loopback(),
                Serialization.JsonRawDeserializeProvider(new JsonSerializerSettings()));
            var test = default(ValueTuple);
            
            var payload = Payload.ToPayload(test, toPayOpt).Unwrap();
            var res = Payload.FromPayload(payload, fromPayOpt).As<ValueTuple>().Unwrap();
            
            Assert.Equal(test.GetType(), res.GetType());
            

        }
    }
}