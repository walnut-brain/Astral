using System;
using System.CodeDom.Compiler;
using Astral.Schema.Generators;
using SchemaTests.Services;
using Xunit;

namespace SchemaTests
{
    public class SchemaGeneration
    {
        [Fact]
        public void MustThrowOnClass()
        {
            var generator = new ServiceSchemaGenerator();
            Assert.Throws<ArgumentException>(() => generator.Generate(typeof(string)));
        }
        
        [Fact]
        public void ServiceHeaderShouldBeCorrect()
        {
            var generator = new ServiceSchemaGenerator();
            var schema = generator.Generate(typeof(ISampleService));
            Assert.Equal("sample", schema.Name);
            Assert.Equal(new Version(1, 0), schema.Version);
            Assert.Equal("SampleService", schema.Title);
        }

        [Fact]
        public void EventsMustBeCollected()
        {
            var generator = new ServiceSchemaGenerator();
            var schema = generator.Generate(typeof(ISampleService));
            Assert.True(schema.Events.ContainsKey("first"));
            Assert.Equal(nameof(ISampleService.SampleEvent), schema.Events["first"].Title);
        }
    }
}