using NUnit.Framework;
using Newtonsoft.Json;

namespace Unions.Tests
{
    [TestFixture]
    public class UnionsSerializationTests
    {
        [SetUp]
        public void SetUp() { }
        [TearDown]
        public void TearDown() { }

        [Test]
        public void Unions_Maybe_AreSerialized() {
            var x = default(Optional<int>);
            Assert.That(JsonConvert.SerializeObject(x), Is.EqualTo("{\"v\":0,\"i\":0}"));
            Assert.That(JsonConvert.DeserializeObject<Optional<int>>(JsonConvert.SerializeObject(x)), Is.EqualTo(x));
            x = None.Value;
            Assert.That(JsonConvert.SerializeObject(x), Is.EqualTo("{\"v\":0,\"i\":0}"));
            Assert.That(JsonConvert.DeserializeObject<Optional<int>>(JsonConvert.SerializeObject(x)), Is.EqualTo(x));
            x = 0;
            Assert.That(JsonConvert.SerializeObject(x), Is.EqualTo("{\"v\":0,\"i\":1}"));
            Assert.That(JsonConvert.DeserializeObject<Optional<int>>(JsonConvert.SerializeObject(x)), Is.EqualTo(x));
            x = 7;
            Assert.That(JsonConvert.SerializeObject(x), Is.EqualTo("{\"v\":7,\"i\":1}"));
            Assert.That(JsonConvert.DeserializeObject<Optional<int>>(JsonConvert.SerializeObject(x)), Is.EqualTo(x));

            var y = default(Optional<string>);
            Assert.That(JsonConvert.SerializeObject(y), Is.EqualTo("{\"v\":null,\"i\":0}"));
            Assert.That(JsonConvert.DeserializeObject<Optional<string>>(JsonConvert.SerializeObject(y)), Is.EqualTo(y));
            y = null;
            Assert.That(JsonConvert.SerializeObject(y), Is.EqualTo("{\"v\":null,\"i\":0}"));
            Assert.That(JsonConvert.DeserializeObject<Optional<string>>(JsonConvert.SerializeObject(y)), Is.EqualTo(y));
            y = new Some<string>(null); 
            Assert.That(JsonConvert.SerializeObject(y), Is.EqualTo("{\"v\":null,\"i\":1}"));
            Assert.That(JsonConvert.DeserializeObject<Optional<string>>(JsonConvert.SerializeObject(y)), Is.EqualTo(y));
            y = y.ToNoneIfNull();
            Assert.That(JsonConvert.SerializeObject(y), Is.EqualTo("{\"v\":null,\"i\":0}"));
            Assert.That(JsonConvert.DeserializeObject<Optional<string>>(JsonConvert.SerializeObject(y)), Is.EqualTo(y));
            y = None.Value;
            Assert.That(JsonConvert.SerializeObject(y), Is.EqualTo("{\"v\":null,\"i\":0}"));
            Assert.That(JsonConvert.DeserializeObject<Optional<string>>(JsonConvert.SerializeObject(y)), Is.EqualTo(y));
            y = "seven";
            Assert.That(JsonConvert.SerializeObject(y), Is.EqualTo("{\"v\":\"seven\",\"i\":1}"));
            Assert.That(JsonConvert.DeserializeObject<Optional<string>>(JsonConvert.SerializeObject(y)), Is.EqualTo(y));
        }
        [Test]
        public void Unions_Either_Is_Serialized() {
            var x = default(Either<int, string>);
            Assert.That(JsonConvert.SerializeObject(x), Is.EqualTo("{\"v\":null,\"i\":null}"));
            Assert.That(JsonConvert.DeserializeObject<Either<int, string>>(JsonConvert.SerializeObject(x)), Is.EqualTo(x));
            x = null;
            Assert.That(JsonConvert.SerializeObject(x), Is.EqualTo("{\"v\":null,\"i\":null}"));
            Assert.That(JsonConvert.DeserializeObject<Either<int, string>>(JsonConvert.SerializeObject(x)), Is.EqualTo(x));
            x = 0;
            Assert.That(JsonConvert.SerializeObject(x), Is.EqualTo("{\"v\":0,\"i\":0}"));
            Assert.That(JsonConvert.DeserializeObject<Either<int, string>>(JsonConvert.SerializeObject(x)), Is.EqualTo(x));
            x = 7;
            Assert.That(JsonConvert.SerializeObject(x), Is.EqualTo("{\"v\":7,\"i\":0}"));
            Assert.That(JsonConvert.DeserializeObject<Either<int, string>>(JsonConvert.SerializeObject(x)), Is.EqualTo(x));
            x = "seven";
            Assert.That(JsonConvert.SerializeObject(x), Is.EqualTo("{\"v\":\"seven\",\"i\":1}"));
            Assert.That(JsonConvert.DeserializeObject<Either<int, string>>(JsonConvert.SerializeObject(x)), Is.EqualTo(x));
        }
        [Test]
        public void Unions_Result_Is_Serialized() {
            var x = default(Result<int, string>);
            Assert.That(JsonConvert.SerializeObject(x), Is.EqualTo("{\"v\":null,\"i\":null}"));
            Assert.That(JsonConvert.DeserializeObject<Result<int, string>>(JsonConvert.SerializeObject(x)), Is.EqualTo(x));
            x = null;
            Assert.That(JsonConvert.SerializeObject(x), Is.EqualTo("{\"v\":null,\"i\":null}"));
            Assert.That(JsonConvert.DeserializeObject<Result<int, string>>(JsonConvert.SerializeObject(x)), Is.EqualTo(x));
            x = 0;
            Assert.That(JsonConvert.SerializeObject(x), Is.EqualTo("{\"v\":0,\"i\":0}"));
            Assert.That(JsonConvert.DeserializeObject<Result<int, string>>(JsonConvert.SerializeObject(x)), Is.EqualTo(x));
            x = 7;
            Assert.That(JsonConvert.SerializeObject(x), Is.EqualTo("{\"v\":7,\"i\":0}"));
            Assert.That(JsonConvert.DeserializeObject<Result<int, string>>(JsonConvert.SerializeObject(x)), Is.EqualTo(x));
            x = "seven";
            Assert.That(JsonConvert.SerializeObject(x), Is.EqualTo("{\"v\":\"seven\",\"i\":1}"));
            Assert.That(JsonConvert.DeserializeObject<Result<int, string>>(JsonConvert.SerializeObject(x)), Is.EqualTo(x));
        }
        [Test]
        public void Unions_AccResult_Is_Serialized() {
            var x = default(AccResult<int, string>);
            Assert.That(JsonConvert.SerializeObject(x), Is.EqualTo("{\"v\":null,\"i\":null}"));
            Assert.That(JsonConvert.DeserializeObject<AccResult<int, string>>(JsonConvert.SerializeObject(x)), Is.EqualTo(x));
            x = 0;
            Assert.That(JsonConvert.SerializeObject(x), Is.EqualTo("{\"v\":0,\"i\":0}"));
            Assert.That(JsonConvert.DeserializeObject<AccResult<int, string>>(JsonConvert.SerializeObject(x)), Is.EqualTo(x));
            x = 7;
            Assert.That(JsonConvert.SerializeObject(x), Is.EqualTo("{\"v\":7,\"i\":0}"));
            Assert.That(JsonConvert.DeserializeObject<AccResult<int, string>>(JsonConvert.SerializeObject(x)), Is.EqualTo(x));

            // why doesn't this work ?
            x = "seven";
            Assert.That(JsonConvert.SerializeObject(x), Is.EqualTo("{\"v\":[\"seven\"],\"i\":1}"));
            Assert.That(JsonConvert.SerializeObject(JsonConvert.DeserializeObject<AccResult<int, string>>(JsonConvert.SerializeObject(x))), Is.EqualTo(JsonConvert.SerializeObject(x))); 
            x = new string[] { "seven", "eight" };
            Assert.That(JsonConvert.SerializeObject(x), Is.EqualTo("{\"v\":[\"seven\",\"eight\"],\"i\":1}"));
            Assert.That(JsonConvert.SerializeObject(JsonConvert.DeserializeObject<AccResult<int, string>>(JsonConvert.SerializeObject(x))), Is.EqualTo(JsonConvert.SerializeObject(x)));
        }
        [Test]
        public void Unions_OneOf3_Is_Serialized() {
            var x = default(OneOf<int, bool, string>);
            Assert.That(JsonConvert.SerializeObject(x), Is.EqualTo("{\"v\":null,\"i\":null}"));
            Assert.That(JsonConvert.DeserializeObject<OneOf<int, bool, string>>(JsonConvert.SerializeObject(x)), Is.EqualTo(x));
            x = null;
            Assert.That(JsonConvert.SerializeObject(x), Is.EqualTo("{\"v\":null,\"i\":null}"));
            Assert.That(JsonConvert.DeserializeObject<OneOf<int, bool, string>>(JsonConvert.SerializeObject(x)), Is.EqualTo(x));
            x = 0;
            Assert.That(JsonConvert.SerializeObject(x), Is.EqualTo("{\"v\":0,\"i\":0}"));
            Assert.That(JsonConvert.DeserializeObject<OneOf<int, bool, string>>(JsonConvert.SerializeObject(x)), Is.EqualTo(x));
            x = 7;
            Assert.That(JsonConvert.SerializeObject(x), Is.EqualTo("{\"v\":7,\"i\":0}"));
            Assert.That(JsonConvert.DeserializeObject<OneOf<int, bool, string>>(JsonConvert.SerializeObject(x)), Is.EqualTo(x));
            x = "seven";
            Assert.That(JsonConvert.SerializeObject(x), Is.EqualTo("{\"v\":\"seven\",\"i\":2}"));
            Assert.That(JsonConvert.DeserializeObject<OneOf<int, bool, string>>(JsonConvert.SerializeObject(x)), Is.EqualTo(x));
        }
        [Test]
        public void Unions_OneOf4_Is_Serialized() {
            var x = default(OneOf<int, bool, char, string>);
            Assert.That(JsonConvert.SerializeObject(x), Is.EqualTo("{\"v\":null,\"i\":null}"));
            Assert.That(JsonConvert.DeserializeObject<OneOf<int, bool, char, string>>(JsonConvert.SerializeObject(x)), Is.EqualTo(x));
            x = null;
            Assert.That(JsonConvert.SerializeObject(x), Is.EqualTo("{\"v\":null,\"i\":null}"));
            Assert.That(JsonConvert.DeserializeObject<OneOf<int, bool, char, string>>(JsonConvert.SerializeObject(x)), Is.EqualTo(x));
            x = 0;
            Assert.That(JsonConvert.SerializeObject(x), Is.EqualTo("{\"v\":0,\"i\":0}"));
            Assert.That(JsonConvert.DeserializeObject<OneOf<int, bool, char, string>>(JsonConvert.SerializeObject(x)), Is.EqualTo(x));
            x = 7;
            Assert.That(JsonConvert.SerializeObject(x), Is.EqualTo("{\"v\":7,\"i\":0}"));
            Assert.That(JsonConvert.DeserializeObject<OneOf<int, bool, char, string>>(JsonConvert.SerializeObject(x)), Is.EqualTo(x));
            x = "seven";
            Assert.That(JsonConvert.SerializeObject(x), Is.EqualTo("{\"v\":\"seven\",\"i\":3}"));
            Assert.That(JsonConvert.DeserializeObject<OneOf<int, bool, char, string>>(JsonConvert.SerializeObject(x)), Is.EqualTo(x));
        }

    }
}
