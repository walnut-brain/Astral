using System.Linq;
using System.Net.Mime;
using System.Text;
using Astral;
using FunEx;
using FunEx.Monads;
using Newtonsoft.Json;
using Xunit;

namespace AstralTests.Payloads
{
    public class SerializationTests
    {
        [Fact]
        public void JsonSerializationTestUtf8()
        {
            var ser = Astral.Payloads.Serialization.Serialization
                .JsonRawSerializeProvider(new JsonSerializerSettings());
            var deser = Astral.Payloads.Serialization.Serialization
                .JsonRawDeserializeProvider(new JsonSerializerSettings());
            var test = new ChieldTestContract
            {
                Name = "123",
                A = 25
            };
            var pair = ser(new ContentType("text/json; charset=" + Encoding.UTF8.WebName)).First()(test).Unwrap();
            Assert.Equal<string>("text/json; charset=utf-8", pair.Item1.ToString());
            var d = (ChieldTestContract) deser(pair.Item1.ToOption()).First()(typeof(ChieldTestContract), pair.Item2).Unwrap();
            Assert.Equal(test.Name, d.Name);
            Assert.Equal(test.A, d.A);
        }
        
        [Fact]
        public void JsonSerializationTest()
        {
            var ser = Astral.Payloads.Serialization.Serialization
                .JsonTextSerializeProvider(new JsonSerializerSettings());
            var deser = Astral.Payloads.Serialization.Serialization
                .JsonTextDeserializeProvider(new JsonSerializerSettings());
            var test = new ChieldTestContract
            {
                Name = "123",
                A = 25
            };
            var pair = ser(new ContentType("text/json")).First()(test).Unwrap();
            Assert.Equal<string>("text/json", pair.Item1.ToString());
            var d = (ChieldTestContract) deser(pair.Item1.ToOption()).First()(typeof(ChieldTestContract), pair.Item2).Unwrap();
            Assert.Equal(test.Name, d.Name);
            Assert.Equal(test.A, d.A);
        }
    }
}