using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using Astral;
using Astral.Payloads.DataContracts;
using FunEx;
using FunEx.Monads;
using Microsoft.Extensions.Logging;
using Xunit;

namespace AstralTests.Payloads.DataContracts
{
    public class Decode
    {
        private readonly ILogger _logger;

        public Decode()
        {
            var factory = new LoggerFactory()
                .AddConsole(true);
            _logger = factory.CreateLogger<Encode>();
        }
        
        [Fact]
        public void ShortMustDecode()
        {
            var cvt = TypeDecoder.WellKnownTypesDecode.Loopback();
            Assert.Equal(typeof(short), cvt(_logger, "i16", ImmutableList<Type>.Empty).Unwrap());
        }
        
        [Fact]
        public void StringMustDecode()
        {
            var cvt = TypeDecoder.WellKnownTypesDecode.Loopback();
            Assert.Equal(typeof(string), cvt(_logger, "string", ImmutableList<Type>.Empty).Unwrap());
        }
        
        [Fact]
        public void UnknownCodeMustThrow()
        {
            var cvt = TypeDecoder.WellKnownTypesDecode.Loopback();
            
            Assert.Throws<InvalidOperationException>(() => cvt(_logger, "version", ImmutableList.Create(typeof(Version))).Unwrap());
        }
        
        [Fact]
        public void AttributedMustDecode()
        {
            var cvt = TypeDecoder.AttributedDecoder<ContractAttribute>(p => p.Name).Loopback();
            Assert.Equal(typeof(TestContract), cvt(_logger, "test.contract", ImmutableList.Create(typeof(TestContract))).Unwrap());
        }
        
        [Fact]
        public void NonAttributedMustThrow()
        {
            var cvt = TypeDecoder.AttributedDecoder<ContractAttribute>(p => p.Name).Loopback();
            Assert.Throws<InvalidOperationException>(() => cvt(_logger, "string", ImmutableList.Create(typeof(string))).Unwrap());
        }
        
        [Fact]
        public void AttributedInheritanceMustDecode()
        {
            var cvt = TypeDecoder.AttributedDecoder<ContractAttribute>(p => p.Name).Loopback();
            Assert.Equal(typeof(ChieldTestContract), cvt(_logger, "chield.test", ImmutableList.Create(typeof(TestContract))).Unwrap());
        }
        
        [Fact]
        public void ArrayOfIntMustDecode()
        {
            var cvt = TypeDecoder.WellKnownTypesDecode.Fallback(TypeDecoder.ArrayDecode)
                .Loopback();
            Assert.Equal(typeof(int[]), cvt(_logger, "i32[]", ImmutableList.Create(typeof(IEnumerable<int>))).Unwrap());
        }
        
        [Fact]
        public void ArrayOfArrayOfIntMustDecode()
        {
            var cvt = TypeDecoder.WellKnownTypesDecode.Fallback(TypeDecoder.ArrayDecode)
                .Loopback();
            Assert.Equal(typeof(int[][]), cvt(_logger, "i32[][]", ImmutableList.Create(typeof(int[][]))).Unwrap());
        }
        
        [Fact]
        public void ArrayOfAttributedMustDecode()
        {
            var cvt = TypeDecoder.Default.Loopback();
            Assert.Equal(typeof(ChieldTestContract[]), cvt(_logger, "chield.test[]", 
                ImmutableList.Create(typeof(TestContract[]))).Unwrap());
        }
        
        [Fact]
        public void ArrayOfUnknownMustThrow()
        {
            var cvt = TypeDecoder.Default.Loopback();
            Assert.Throws<InvalidOperationException>(() => cvt(_logger, "version[]",
                ImmutableList.Create(typeof(Version[]))).Unwrap());
        }
    }
}