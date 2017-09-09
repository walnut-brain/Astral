using System;
using System.Linq;
using Astral;
using Astral.Payloads.DataContracts;
using FunEx;
using FunEx.Monads;
using Microsoft.Extensions.Logging;
using Xunit;

namespace AstralTests.Payloads.DataContracts
{
    public class Encode
    {
        private readonly ILogger _logger;

        public Encode()
        {
            var factory = new LoggerFactory()
                .AddConsole(true);
            _logger = factory.CreateLogger<Encode>();
        }

        [Fact]
        public void ShortMustEncode()
        {
            var cvt = TypeEncoder.WellKnownTypesEncode.Loopback();
            Assert.Equal("i16",  cvt(_logger, typeof(short)).Unwrap());
        }
        
        [Fact]
        public void StringMustEncode()
        {
            var cvt = TypeEncoder.WellKnownTypesEncode.Loopback();
            Assert.Equal("string", cvt(_logger,typeof(string)).Unwrap());
        }
        
        [Fact]
        public void UnknownTypeMustThrow()
        {
            var cvt = TypeEncoder.WellKnownTypesEncode.Loopback();
            
            Assert.Throws<InvalidOperationException>(() => cvt(_logger, typeof(TestContract)).Unwrap());
        }

        [Fact]
        public void AttributedMustEncode()
        {
            var cvt = TypeEncoder.AttributedEncoder<ContractAttribute>(p => p.Name).Loopback();
            Assert.Equal("test.contract", cvt(_logger, typeof(TestContract)).Unwrap());
        }
        
        [Fact]
        public void NonAttributedMustThrow()
        {
            var cvt = TypeEncoder.AttributedEncoder<ContractAttribute>(p => p.Name).Loopback();
            Assert.Throws<InvalidOperationException>(() => cvt(_logger, typeof(string)).Unwrap());
        }
        
        [Fact]
        public void AttributedInheritanceMustTakeConcrete()
        {
            var cvt = TypeEncoder.AttributedEncoder<ContractAttribute>(p => p.Name).Loopback();
            Assert.Equal("chield.test", cvt(_logger, typeof(ChieldTestContract)).Unwrap());
        }

        [Fact]
        public void ArrayOfIntMustEncode()
        {
            var cvt = TypeEncoder.WellKnownTypesEncode.Fallback(TypeEncoder.ArrayLikeEncode)
                .Loopback();
            Assert.Equal("i32[]", cvt(_logger, typeof(int[])).Unwrap());
        }
        
        [Fact]
        public void ArrayOfArrayOfIntMustEncode()
        {
            var cvt = TypeEncoder.WellKnownTypesEncode.Fallback(TypeEncoder.ArrayLikeEncode)
                .Loopback();
            Assert.Equal("i32[][]", cvt(_logger, typeof(int[][])).Unwrap());
        }

        [Fact]
        public void ArrayOfAttributedMustEncode()
        {
            var cvt = TypeEncoder.Default.Loopback();
            Assert.Equal("chield.test[]", cvt(_logger, typeof(ChieldTestContract[])).Unwrap());
        }
        
        [Fact]
        public void ArrayOfUnknownMustThrow()
        {
            var cvt = TypeEncoder.Default.Loopback();
            Assert.Throws<InvalidOperationException>(() => cvt(_logger, typeof(Version[])).Unwrap());
        }

        
    }
}