using System;
using System.Collections.Generic;
using Astral;
using Astral.Payloads.DataContracts;
using LanguageExt;
using Xunit;

namespace AstralTests.Payloads.DataContracts
{
    public class ToType
    {
        [Fact]
        public void WellKnownTypesShort()
        {
            var cvt = Contract.WellKnownContractMapper(WellKnownTypes.Default);
            Assert.Equal(typeof(short), cvt("i16", Seq<Type>.Empty).Unwrap());
        }
        
        [Fact]
        public void WellKnownTypesString()
        {
            var cvt = Contract.WellKnownContractMapper(WellKnownTypes.Default);
            Assert.Equal(typeof(string), cvt("string", Seq<Type>.Empty).Unwrap());
        }
        
        [Fact]
        public void WellKnownTypesThrow()
        {
            var cvt = Contract.WellKnownContractMapper(WellKnownTypes.Default);
            
            Assert.Throws<ContractToTypeException>(() => cvt("version", typeof(Version).Cons()).Unwrap());
        }
        
        [Fact]
        public void AttributeTest()
        {
            var cvt = Contract.AttributeContractMapper.Loopback();
            Assert.Equal(typeof(TestContract), cvt("test.contract", typeof(TestContract).Cons()).Unwrap());
        }
        
        [Fact]
        public void AttributeThrow()
        {
            var cvt = Contract.AttributeContractMapper.Loopback();
            Assert.Throws<ContractToTypeException>(() => cvt("string", typeof(string).Cons()).Unwrap());
        }
        
        [Fact]
        public void AttributeChield()
        {
            var cvt = Contract.AttributeContractMapper.Loopback();
            Assert.Equal(typeof(ChieldTestContract), cvt("chield.test", typeof(TestContract).Cons()).Unwrap());
        }
        
        [Fact]
        public void ArrayWellKnownInt()
        {
            var cvt = Contract.WellKnownContractMapper(WellKnownTypes.Default).Fallback(Contract.ArrayContractMapper)
                .Loopback();
            Assert.Equal(typeof(int[]), cvt("i32[]", typeof(IEnumerable<int>).Cons()).Unwrap());
        }
        
        [Fact]
        public void Array2WellKnownInt()
        {
            var cvt = Contract.WellKnownContractMapper(WellKnownTypes.Default).Fallback(Contract.ArrayContractMapper)
                .Loopback();
            Assert.Equal(typeof(int[][]), cvt("i32[][]", typeof(int[][]).Cons()).Unwrap());
        }
        
        [Fact]
        public void DefaultTest()
        {
            var cvt = Contract.DefaultContractMapper(WellKnownTypes.Default).Loopback();
            Assert.Equal(typeof(ChieldTestContract[]), cvt("chield.test[]", typeof(TestContract[]).Cons()).Unwrap());
        }
        
        [Fact]
        public void DefaultThrow()
        {
            var cvt = Contract.DefaultContractMapper(WellKnownTypes.Default).Loopback();
            Assert.Throws<ContractToTypeException>(() => cvt("version[]",typeof(Version[]).Cons()).Unwrap());
        }
    }
}