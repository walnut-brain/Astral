using System.Net.Mime;
using Astral;
using Astral.Payloads;
using Astral.Payloads.DataContracts;
using Astral.Payloads.Serialization;
using Newtonsoft.Json;
using Xunit;
using System;
using System.Diagnostics.Contracts;
using FunEx;
using FunEx.Monads;
using Microsoft.Extensions.Logging;

namespace AstralTests.Payloads
{
    public class PayloadTests
    {
        private readonly ILogger _logger;

        

        public PayloadTests()
        {
            var factory = new LoggerFactory()
                .AddConsole(true);
            _logger = factory.CreateLogger<Encode>();            
        }

        [Fact]
        public void ComplexTypeMustWork()
        {
            var toPayOpt = new ToPayloadOptions<byte[]>(new ContentType("text/json; charset=utf-8"),
                TypeEncoding.Default.Encode,
                Serialization.JsonRawSerializeProvider(new JsonSerializerSettings()));
            var fromPayOpt = new FromPayloadOptions<byte[]>(TypeEncoding.Default.Decode,
                Serialization.JsonRawDeserializeProvider(new JsonSerializerSettings()));
            var test = new TestContract[]
            {
                new ChieldTestContract { Name = "123", A = 5 },
                new ChieldTestContract { Name = "234", A = 6}, 
            };
            var payload = Payload.ToPayload(_logger, test, toPayOpt).Unwrap();
            var res = Payload.FromPayload(_logger, payload, fromPayOpt).As<TestContract[]>().Unwrap();
            
            Assert.Equal(test.GetType(), res.GetType());
            Assert.Equal(test[1].Name, res[1].Name);

        }
        
        [Fact]
        public void ValueTupleMustWork()
        {
            var toPayOpt = new ToPayloadOptions<byte[]>(new ContentType("text/json; charset=utf-8"),
                TypeEncoding.Default.Encode,
                Serialization.JsonRawSerializeProvider(new JsonSerializerSettings()));
            var fromPayOpt = new FromPayloadOptions<byte[]>(TypeEncoding.Default.Decode,
                Serialization.JsonRawDeserializeProvider(new JsonSerializerSettings()));
            var test = default(ValueTuple);
            
            var payload = Payload.ToPayload(_logger, test, toPayOpt).Unwrap();
            var res = Payload.FromPayload(_logger, payload, fromPayOpt).As<ValueTuple>().Unwrap();
            
            Assert.Equal(test.GetType(), res.GetType());
            

        }
        
        [Fact]
        public void ValueTupleToTextMustWork()
        {
            var toPayOpt = new ToPayloadOptions<string>(new ContentType("text/json"),
                TypeEncoding.Default.Encode,
                Serialization.JsonTextSerializeProvider(new JsonSerializerSettings()));
            var fromPayOpt = new FromPayloadOptions<string>(TypeEncoding.Default.Decode,
                Serialization.JsonTextDeserializeProvider(new JsonSerializerSettings()));
            var test = default(ValueTuple);
            
            var payload = Payload.ToPayload(_logger, test, toPayOpt).Unwrap();
            Assert.Equal("unit", payload.TypeCode);
            Assert.Equal("{}", payload.Data);
            var res = Payload.FromPayload(_logger, payload, fromPayOpt).As<ValueTuple>().Unwrap();
            
            Assert.Equal(test.GetType(), res.GetType());
            

        }
    }
}