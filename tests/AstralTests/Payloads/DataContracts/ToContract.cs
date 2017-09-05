using System;
using Astral;
using Astral.Payloads.DataContracts;
using Xunit;

namespace AstralTests.Payloads.DataContracts
{
    public class ToContract
    {
        [Fact]
        public void WellKnownTypesShort()
        {
            var cvt = Contract.WellKnownTypeMapper(WellKnownTypes.Default);
            Assert.Equal("i16", cvt(typeof(short)).Unwrap());
        }
        
        [Fact]
        public void WellKnownTypesString()
        {
            var cvt = Contract.WellKnownTypeMapper(WellKnownTypes.Default);
            Assert.Equal("string", cvt(typeof(string)).Unwrap());
        }
        
        [Fact]
        public void WellKnownTypesThrow()
        {
            var cvt = Contract.WellKnownTypeMapper(WellKnownTypes.Default);
            
            Assert.Throws<TypeToContractException>(() => cvt(typeof(TestContract)).Unwrap());
        }

        [Fact]
        public void AttributeTest()
        {
            var cvt = Contract.AttributeTypeMapper;
            Assert.Equal("test.contract", cvt(typeof(TestContract)).Unwrap());
        }
        
        [Fact]
        public void AttributeThrow()
        {
            var cvt = Contract.AttributeTypeMapper;
            Assert.Throws<TypeToContractException>(() => cvt(typeof(string)).Unwrap());
        }
        
        [Fact]
        public void AttributeChield()
        {
            var cvt = Contract.AttributeTypeMapper;
            Assert.Equal("chield.test", cvt(typeof(ChieldTestContract)).Unwrap());
        }

        [Fact]
        public void ArrayWellKnownInt()
        {
            var cvt = Contract.WellKnownTypeMapper(WellKnownTypes.Default).Fallback(Contract.ArrayLikeTypeMapper)
                .Loopback();
            Assert.Equal("i32[]", cvt(typeof(int[])).Unwrap());
        }
        
        [Fact]
        public void Array2WellKnownInt()
        {
            var cvt = Contract.WellKnownTypeMapper(WellKnownTypes.Default).Fallback(Contract.ArrayLikeTypeMapper)
                .Loopback();
            Assert.Equal("i32[][]", cvt(typeof(int[][])).Unwrap());
        }

        [Fact]
        public void DefaultTest()
        {
            var cvt = Contract.DefaultTypeMapper(WellKnownTypes.Default).Loopback();
            Assert.Equal("chield.test[]", cvt(typeof(ChieldTestContract[])).Unwrap());
        }
        
        [Fact]
        public void DefaultThrow()
        {
            var cvt = Contract.DefaultTypeMapper(WellKnownTypes.Default).Loopback();
            Assert.Throws<TypeToContractException>(() => cvt(typeof(Version[])).Unwrap());
        }
    }
}