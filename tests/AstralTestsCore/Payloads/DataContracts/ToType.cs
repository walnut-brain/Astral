using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Astral.Payloads.DataContracts;
using FunEx;
using FunEx.Monads;
using Xunit;

namespace AstralTests.Payloads.DataContracts
{
    public class ToType
    {
        [Fact]
        public void WellKnownTypesShort()
        {
            var cvt = Contract.WellKnownContractMapper(WellKnownTypes.Default);
            Assert.Equal(typeof(short), cvt("i16", ImmutableList<Type>.Empty).Unwrap());
        }
        
        [Fact]
        public void WellKnownTypesString()
        {
            var cvt = Contract.WellKnownContractMapper(WellKnownTypes.Default);
            Assert.Equal(typeof(string), cvt("string", ImmutableList<Type>.Empty).Unwrap());
        }
        
        [Fact]
        public void WellKnownTypesThrow()
        {
            var cvt = Contract.WellKnownContractMapper(WellKnownTypes.Default);
            
            Assert.Throws<ContractToTypeException>(() => cvt("version", ImmutableList.Create(typeof(Version))).Unwrap());
        }
        
        [Fact]
        public void AttributeTest()
        {
            var cvt = Contract.AttributeContractMapper.Loopback();
            Assert.Equal(typeof(TestContract), cvt("test.contract", ImmutableList.Create(typeof(TestContract))).Unwrap());
        }
        
        [Fact]
        public void AttributeThrow()
        {
            var cvt = Contract.AttributeContractMapper.Loopback();
            Assert.Throws<ContractToTypeException>(() => cvt("string", ImmutableList.Create(typeof(string))).Unwrap());
        }
        
        [Fact]
        public void AttributeChield()
        {
            var cvt = Contract.AttributeContractMapper.Loopback();
            Assert.Equal(typeof(ChieldTestContract), cvt("chield.test", ImmutableList.Create(typeof(TestContract))).Unwrap());
        }
        
        [Fact]
        public void ArrayWellKnownInt()
        {
            var cvt = Contract.WellKnownContractMapper(WellKnownTypes.Default).Fallback(Contract.ArrayContractMapper)
                .Loopback();
            Assert.Equal(typeof(int[]), cvt("i32[]", ImmutableList.Create(typeof(IEnumerable<int>))).Unwrap());
        }
        
        [Fact]
        public void Array2WellKnownInt()
        {
            var cvt = Contract.WellKnownContractMapper(WellKnownTypes.Default).Fallback(Contract.ArrayContractMapper)
                .Loopback();
            Assert.Equal(typeof(int[][]), cvt("i32[][]", ImmutableList.Create(typeof(int[][]))).Unwrap());
        }
        
        [Fact]
        public void DefaultTest()
        {
            var cvt = Contract.DefaultContractMapper(WellKnownTypes.Default).Loopback();
            Assert.Equal(typeof(ChieldTestContract[]), cvt("chield.test[]", 
                ImmutableList.Create(typeof(TestContract[]))).Unwrap());
        }
        
        [Fact]
        public void DefaultThrow()
        {
            var cvt = Contract.DefaultContractMapper(WellKnownTypes.Default).Loopback();
            Assert.Throws<ContractToTypeException>(() => cvt("version[]",
                ImmutableList.Create(typeof(Version[]))).Unwrap());
        }
    }
}