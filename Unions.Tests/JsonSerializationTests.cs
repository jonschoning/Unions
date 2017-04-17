using Xunit;
using Newtonsoft.Json;

using Unions;

namespace Unions.Tests
{
    public class UnionsSerializationTests
    {

        [Fact]
        public void Unions_Maybe_AreSerialized() {
            var x = default(Optional<int>);
            Assert.Equal(JsonConvert.SerializeObject(x), ("{\"v\":0,\"i\":0}"));
            Assert.Equal(JsonConvert.DeserializeObject<Optional<int>>(JsonConvert.SerializeObject(x)), (x));
            x = None.Value;
            Assert.Equal(JsonConvert.SerializeObject(x), ("{\"v\":0,\"i\":0}"));
            Assert.Equal(JsonConvert.DeserializeObject<Optional<int>>(JsonConvert.SerializeObject(x)), (x));
            x = 0;
            Assert.Equal(JsonConvert.SerializeObject(x), ("{\"v\":0,\"i\":1}"));
            Assert.Equal(JsonConvert.DeserializeObject<Optional<int>>(JsonConvert.SerializeObject(x)), (x));
            x = 7;
            Assert.Equal(JsonConvert.SerializeObject(x), ("{\"v\":7,\"i\":1}"));
            Assert.Equal(JsonConvert.DeserializeObject<Optional<int>>(JsonConvert.SerializeObject(x)), (x));

            var y = default(Optional<string>);
            Assert.Equal(JsonConvert.SerializeObject(y), ("{\"v\":null,\"i\":0}"));
            Assert.Equal(JsonConvert.DeserializeObject<Optional<string>>(JsonConvert.SerializeObject(y)), (y));
            y = null;
            Assert.Equal(JsonConvert.SerializeObject(y), ("{\"v\":null,\"i\":0}"));
            Assert.Equal(JsonConvert.DeserializeObject<Optional<string>>(JsonConvert.SerializeObject(y)), (y));
            y = new Some<string>(null); 
            Assert.Equal(JsonConvert.SerializeObject(y), ("{\"v\":null,\"i\":1}"));
            Assert.Equal(JsonConvert.DeserializeObject<Optional<string>>(JsonConvert.SerializeObject(y)), (y));
            y = y.ToNoneIfNull();
            Assert.Equal(JsonConvert.SerializeObject(y), ("{\"v\":null,\"i\":0}"));
            Assert.Equal(JsonConvert.DeserializeObject<Optional<string>>(JsonConvert.SerializeObject(y)), (y));
            y = None.Value;
            Assert.Equal(JsonConvert.SerializeObject(y), ("{\"v\":null,\"i\":0}"));
            Assert.Equal(JsonConvert.DeserializeObject<Optional<string>>(JsonConvert.SerializeObject(y)), (y));
            y = "seven";
            Assert.Equal(JsonConvert.SerializeObject(y), ("{\"v\":\"seven\",\"i\":1}"));
            Assert.Equal(JsonConvert.DeserializeObject<Optional<string>>(JsonConvert.SerializeObject(y)), (y));
        }
        [Fact]
        public void Unions_Either_Is_Serialized() {
            var x = default(Either<int, string>);
            Assert.Equal(JsonConvert.SerializeObject(x), ("{\"v\":null,\"i\":null}"));
            Assert.Equal(JsonConvert.DeserializeObject<Either<int, string>>(JsonConvert.SerializeObject(x)), (x));
            x = null;
            Assert.Equal(JsonConvert.SerializeObject(x), ("{\"v\":null,\"i\":null}"));
            Assert.Equal(JsonConvert.DeserializeObject<Either<int, string>>(JsonConvert.SerializeObject(x)), (x));
            x = 0;
            Assert.Equal(JsonConvert.SerializeObject(x), ("{\"v\":0,\"i\":0}"));
            Assert.Equal(JsonConvert.DeserializeObject<Either<int, string>>(JsonConvert.SerializeObject(x)), (x));
            x = 7;
            Assert.Equal(JsonConvert.SerializeObject(x), ("{\"v\":7,\"i\":0}"));
            Assert.Equal(JsonConvert.DeserializeObject<Either<int, string>>(JsonConvert.SerializeObject(x)), (x));
            x = "seven";
            Assert.Equal(JsonConvert.SerializeObject(x), ("{\"v\":\"seven\",\"i\":1}"));
            Assert.Equal(JsonConvert.DeserializeObject<Either<int, string>>(JsonConvert.SerializeObject(x)), (x));
        }
        [Fact]
        public void Unions_Result_Is_Serialized() {
            var x = default(Result<int, string>);
            Assert.Equal(JsonConvert.SerializeObject(x), ("{\"v\":null,\"i\":null}"));
            Assert.Equal(JsonConvert.DeserializeObject<Result<int, string>>(JsonConvert.SerializeObject(x)), (x));
            x = null;
            Assert.Equal(JsonConvert.SerializeObject(x), ("{\"v\":null,\"i\":null}"));
            Assert.Equal(JsonConvert.DeserializeObject<Result<int, string>>(JsonConvert.SerializeObject(x)), (x));
            x = 0;
            Assert.Equal(JsonConvert.SerializeObject(x), ("{\"v\":0,\"i\":0}"));
            Assert.Equal(JsonConvert.DeserializeObject<Result<int, string>>(JsonConvert.SerializeObject(x)), (x));
            x = 7;
            Assert.Equal(JsonConvert.SerializeObject(x), ("{\"v\":7,\"i\":0}"));
            Assert.Equal(JsonConvert.DeserializeObject<Result<int, string>>(JsonConvert.SerializeObject(x)), (x));
            x = "seven";
            Assert.Equal(JsonConvert.SerializeObject(x), ("{\"v\":\"seven\",\"i\":1}"));
            Assert.Equal(JsonConvert.DeserializeObject<Result<int, string>>(JsonConvert.SerializeObject(x)), (x));
        }
        [Fact]
        public void Unions_AccResult_Is_Serialized() {
            var x = default(AccResult<int, string>);
            Assert.Equal(JsonConvert.SerializeObject(x), ("{\"v\":null,\"i\":null}"));
            Assert.Equal(JsonConvert.DeserializeObject<AccResult<int, string>>(JsonConvert.SerializeObject(x)), (x));
            x = 0;
            Assert.Equal(JsonConvert.SerializeObject(x), ("{\"v\":0,\"i\":0}"));
            Assert.Equal(JsonConvert.DeserializeObject<AccResult<int, string>>(JsonConvert.SerializeObject(x)), (x));
            x = 7;
            Assert.Equal(JsonConvert.SerializeObject(x), ("{\"v\":7,\"i\":0}"));
            Assert.Equal(JsonConvert.DeserializeObject<AccResult<int, string>>(JsonConvert.SerializeObject(x)), (x));

            // why doesn't this work ?
            x = "seven";
            Assert.Equal(JsonConvert.SerializeObject(x), ("{\"v\":[\"seven\"],\"i\":1}"));
            Assert.Equal(JsonConvert.SerializeObject(JsonConvert.DeserializeObject<AccResult<int, string>>(JsonConvert.SerializeObject(x))), (JsonConvert.SerializeObject(x))); 
            x = new string[] { "seven", "eight" };
            Assert.Equal(JsonConvert.SerializeObject(x), ("{\"v\":[\"seven\",\"eight\"],\"i\":1}"));
            Assert.Equal(JsonConvert.SerializeObject(JsonConvert.DeserializeObject<AccResult<int, string>>(JsonConvert.SerializeObject(x))), (JsonConvert.SerializeObject(x)));
        }
        [Fact]
        public void Unions_OneOf3_Is_Serialized() {
            var x = default(OneOf<int, bool, string>);
            Assert.Equal(JsonConvert.SerializeObject(x), ("{\"v\":null,\"i\":null}"));
            Assert.Equal(JsonConvert.DeserializeObject<OneOf<int, bool, string>>(JsonConvert.SerializeObject(x)), (x));
            x = null;
            Assert.Equal(JsonConvert.SerializeObject(x), ("{\"v\":null,\"i\":null}"));
            Assert.Equal(JsonConvert.DeserializeObject<OneOf<int, bool, string>>(JsonConvert.SerializeObject(x)), (x));
            x = 0;
            Assert.Equal(JsonConvert.SerializeObject(x), ("{\"v\":0,\"i\":0}"));
            Assert.Equal(JsonConvert.DeserializeObject<OneOf<int, bool, string>>(JsonConvert.SerializeObject(x)), (x));
            x = 7;
            Assert.Equal(JsonConvert.SerializeObject(x), ("{\"v\":7,\"i\":0}"));
            Assert.Equal(JsonConvert.DeserializeObject<OneOf<int, bool, string>>(JsonConvert.SerializeObject(x)), (x));
            x = "seven";
            Assert.Equal(JsonConvert.SerializeObject(x), ("{\"v\":\"seven\",\"i\":2}"));
            Assert.Equal(JsonConvert.DeserializeObject<OneOf<int, bool, string>>(JsonConvert.SerializeObject(x)), (x));
        }
        [Fact]
        public void Unions_OneOf4_Is_Serialized() {
            var x = default(OneOf<int, bool, char, string>);
            Assert.Equal(JsonConvert.SerializeObject(x), ("{\"v\":null,\"i\":null}"));
            Assert.Equal(JsonConvert.DeserializeObject<OneOf<int, bool, char, string>>(JsonConvert.SerializeObject(x)), (x));
            x = null;
            Assert.Equal(JsonConvert.SerializeObject(x), ("{\"v\":null,\"i\":null}"));
            Assert.Equal(JsonConvert.DeserializeObject<OneOf<int, bool, char, string>>(JsonConvert.SerializeObject(x)), (x));
            x = 0;
            Assert.Equal(JsonConvert.SerializeObject(x), ("{\"v\":0,\"i\":0}"));
            Assert.Equal(JsonConvert.DeserializeObject<OneOf<int, bool, char, string>>(JsonConvert.SerializeObject(x)), (x));
            x = 7;
            Assert.Equal(JsonConvert.SerializeObject(x), ("{\"v\":7,\"i\":0}"));
            Assert.Equal(JsonConvert.DeserializeObject<OneOf<int, bool, char, string>>(JsonConvert.SerializeObject(x)), (x));
            x = "seven";
            Assert.Equal(JsonConvert.SerializeObject(x), ("{\"v\":\"seven\",\"i\":3}"));
            Assert.Equal(JsonConvert.DeserializeObject<OneOf<int, bool, char, string>>(JsonConvert.SerializeObject(x)), (x));
        }

    }
}
