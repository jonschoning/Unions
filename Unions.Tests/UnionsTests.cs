using Xunit;
using System.Collections.Generic;
using System;

using Unions;

namespace Unions.Tests
{
    public class UnionsTests
    {
        [Fact]
        public void Unions_Maybe_HasValues() {

            // value types
            var x = default(Optional<int>);
            Assert.True(x.IsNone);
            Assert.False(x.IsSomeT);
            Assert.False(x.IsSomeNull);
            x = None.Value;
            Assert.True(x.IsNone);
            Assert.False(x.IsSomeT);
            Assert.False(x.IsSomeNull);
            x = 0;
            Assert.True(x.IsSomeT);
            Assert.False(x.IsNone);
            Assert.False(x.IsSomeNull);
            x = 7;
            Assert.True(x.IsSomeT);
            Assert.False(x.IsNone);
            Assert.False(x.IsSomeNull);

            // reference types
            var y = default(Optional<string>);
            Assert.True(y.IsNone);
            Assert.False(y.IsSomeT);
            Assert.False(y.IsSomeNull);
            y = null;                  // implicit convertion to None
            Assert.True(y.IsNone);
            Assert.False(y.IsSomeT);
            Assert.False(y.IsSomeNull);
            y = Optional<string>.FromSome("7");
            Assert.False(y.IsNone);
            Assert.True(y.IsSomeT);
            Assert.False(y.IsSomeNull);   
            y = new Some<string>(null); // explicit Some(null)
            Assert.False(y.IsNone);
            Assert.True(y.IsSomeT);
            Assert.True(y.IsSomeNull);   
            y = Optional<string>.FromSome(null); // explicit Some(null)
            Assert.False(y.IsNone);
            Assert.True(y.IsSomeT);
            Assert.True(y.IsSomeNull);   
            y = y.ToNoneIfNull();
            Assert.True(y.IsNone);
            Assert.False(y.IsSomeT);
            Assert.False(y.IsSomeNull);
            y = None.Value;
            Assert.True(y.IsNone);
            Assert.False(y.IsSomeT);
            Assert.False(y.IsSomeNull);
            y = string.Empty;
            Assert.True(y.IsSomeT);
            Assert.False(y.IsNone);
            Assert.False(y.IsSomeNull);
            y = "seven";
            Assert.True(y.IsSomeT);
            Assert.False(y.IsNone);
            Assert.False(y.IsSomeNull);
        }
        [Fact]
        public void Unions_Result__HasValues() {
            var x = default(Result<int, string>);
            Assert.False(x.HasValue);
            Assert.False(x.IsSuccess);
            Assert.False(x.IsFailure);
            x = null;
            Assert.False(x.HasValue);
            Assert.False(x.IsSuccess);
            Assert.False(x.IsFailure);
            x = 0;
            Assert.True(x.HasValue);
            Assert.True(x.IsSuccess);
            Assert.False(x.IsFailure);
            Assert.Equal(x.AsSuccess, (0));
            x = 7;
            Assert.True(x.HasValue);
            Assert.True(x.IsSuccess);
            Assert.False(x.IsFailure);
            Assert.Equal(x.AsSuccess, (7));
            x = "seven";
            Assert.True(x.HasValue);
            Assert.False(x.IsSuccess);
            Assert.True(x.IsFailure);
            Assert.Equal(x.AsFailure, ("seven"));
        }


        [Fact]
        public void Unions_AccResult__HasValues() {
            var x = default(AccResult<int, string>);
            Assert.False(x.HasValue);
            Assert.False(x.IsSuccess);
            Assert.False(x.IsFailure);
            x = 0;
            Assert.True(x.HasValue);
            Assert.True(x.IsSuccess);
            Assert.False(x.IsFailure);
            Assert.Equal(x.AsSuccess, (0));
            x = 7;
            Assert.True(x.HasValue);
            Assert.True(x.IsSuccess);
            Assert.False(x.IsFailure);
            Assert.Equal(x.AsSuccess, (7));
            x = "seven";
            Assert.True(x.HasValue);
            Assert.False(x.IsSuccess);
            Assert.True(x.IsFailure);
            Assert.Equal(x.AsFailure, ((new List<string>() { "seven" })));

            var isEven = new AccResult<Func<int, bool>, string>(_ => _ % 2 == 0);
            var ret = isEven.Apply(x);
            Assert.True(ret.HasValue);
            Assert.False(ret.IsSuccess);
            Assert.True(ret.IsFailure);
            Assert.Equal(ret.AsFailure, ((new List<string>() { "seven" })));

            x = 8;
            ret = isEven.Apply(x);
            Assert.True(ret.HasValue);
            Assert.True(ret.IsSuccess);
            Assert.False(ret.IsFailure);
            Assert.Equal(ret.AsSuccess, (true));

            var hasFailure = new AccResult<Func<int, bool>, string>("failure");
            ret = hasFailure.Apply(x);
            Assert.True(ret.HasValue);
            Assert.False(ret.IsSuccess);
            Assert.True(ret.IsFailure);
            Assert.Equal(ret.AsFailure, ((new List<string>() { "failure" })));

            x = "bothfailure";
            ret = hasFailure.Apply(x);
            Assert.True(ret.HasValue);
            Assert.False(ret.IsSuccess);
            Assert.True(ret.IsFailure);
            Assert.Equal(ret.AsFailure, ((new List<string>() { "failure", "bothfailure" })));

            var ret2 = hasFailure.Apply(x);
            Assert.Equal(ret.AsFailure, (ret2.AsFailure));

            // apply (success)
            var a = new AccResult<int,string>(3);
            var b = new AccResult<int,string>(4);
            var c = new AccResult<int,string>(5);
            var applyResult = AccResultOps.LiftA3(Tuple.Create, a, b, c);
            var expected = Tuple.Create(3, 4, 5);
            Assert.Equal(applyResult.AsSuccess, (expected));


            // apply (failure)
            var a2 = new AccResult<int,string>(3);
            var b2 = new AccResult<int,string>("failure 1");
            var c2 = new AccResult<int,string>("failure 2");
            var applyResult2 = AccResultOps.LiftA3(Tuple.Create, a2, b2, c2);
            var expected2 = new List<string>() { "failure 1", "failure 2" };
            Assert.Equal(applyResult2.AsFailure, (expected2));

            // sequence (success)
            var accList = new AccResult<int, string>[] { 3, 4, 5, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,  3, 4, 5, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            var sequenceResult = accList.SequenceA();
            var successExpected = new int[] { 3, 4, 5, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,  3, 4, 5, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            Assert.Equal(sequenceResult.AsSuccess, (successExpected));

            // sequence (fail)
            var accList2 = new AccResult<int, string>[] { 4, 5, 1, 1, 1, 13, "fail 1", 3, 1, "fail 2", 4, 5, 1, 5, "fail 3" };
            var sequenceResult2 = accList2.SequenceA();
            var failExpected2 = new string[] { "fail 1", "fail 2", "fail 3" };
            Assert.Equal(sequenceResult2.AsFailure, (failExpected2));

            // traverse (success)
            var tList = new int[] { 3, 4, 5, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,  3, 4, 5, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            var tResult = AccResultOps.Traverse<int,int,string>(p1 => p1, tList);
            Assert.Equal(tResult.AsSuccess, (tList));

            // traverse (fail)
            var t2List = new int[] { 3, 4, 5, 1, 1, 1, 1, 1, 1,  3, 6, 5, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            var t2Result = AccResultOps.Traverse<int,int,string>(p1 => {  if (p1 % 2 == 1) { return p1; } else { return "n"+p1.ToString(); }; }, t2List);
            Assert.Equal(t2Result.AsFailure, (new string[]{ "n4", "n6" }));
        }

        [Fact]
        public void Unions_Either_HasValues() {
            var x = default(Either<int, string>);
            Assert.False(x.HasValue);
            Assert.False(x.IsT0);
            Assert.False(x.IsT1);
            x = null;
            Assert.False(x.HasValue);
            Assert.False(x.IsT0);
            Assert.False(x.IsT1);
            x = 0;
            Assert.True(x.HasValue);
            Assert.True(x.IsT0);
            Assert.False(x.IsT1);
            Assert.Equal(x.AsT0, (0));
            x = 7;
            Assert.True(x.HasValue);
            Assert.True(x.IsT0);
            Assert.False(x.IsT1);
            Assert.Equal(x.AsT0, (7));
            x = "seven";
            Assert.True(x.HasValue);
            Assert.False(x.IsT0);
            Assert.True(x.IsT1);
            Assert.Equal(x.AsT1, ("seven"));
        }
        [Fact]
        public void Unions_OneOf3_HasValues() {
            var x = default(OneOf<int, bool, string>);
            Assert.False(x.HasValue);
            Assert.False(x.IsT0);
            Assert.False(x.IsT1);
            Assert.False(x.IsT2);
            x = null;
            Assert.False(x.HasValue);
            Assert.False(x.IsT0);
            Assert.False(x.IsT1);
            Assert.False(x.IsT2);
            x = 0;
            Assert.True(x.HasValue);
            Assert.True(x.IsT0);
            Assert.False(x.IsT1);
            Assert.False(x.IsT2);
            Assert.Equal(x.AsT0, (0));
            x = 7;
            Assert.True(x.HasValue);
            Assert.True(x.IsT0);
            Assert.False(x.IsT1);
            Assert.False(x.IsT2);
            Assert.Equal(x.AsT0, (7));
            x = false;
            Assert.True(x.HasValue);
            Assert.False(x.IsT0);
            Assert.True(x.IsT1);
            Assert.False(x.IsT2);
            Assert.Equal(x.AsT1, (false));
            x = "seven";
            Assert.True(x.HasValue);
            Assert.False(x.IsT0);
            Assert.False(x.IsT1);
            Assert.True(x.IsT2);
            Assert.Equal(x.AsT2, ("seven"));
        }
        [Fact]
        public void Unions_OneOf4_HasValues() {
            var x = default(OneOf<int, bool, char, string>);
            Assert.False(x.HasValue);
            Assert.False(x.IsT0);
            Assert.False(x.IsT1);
            Assert.False(x.IsT2);
            Assert.False(x.IsT3);
            x = null;
            Assert.False(x.HasValue);
            Assert.False(x.IsT0);
            Assert.False(x.IsT1);
            Assert.False(x.IsT2);
            Assert.False(x.IsT3);
            x = 0;
            Assert.True(x.HasValue);
            Assert.True(x.IsT0);
            Assert.False(x.IsT1);
            Assert.False(x.IsT2);
            Assert.False(x.IsT3);
            Assert.Equal(x.AsT0, (0));
            x = 7;
            Assert.True(x.HasValue);
            Assert.True(x.IsT0);
            Assert.False(x.IsT1);
            Assert.False(x.IsT2);
            Assert.False(x.IsT3);
            Assert.Equal(x.AsT0, (7));
            x = false;
            Assert.True(x.HasValue);
            Assert.False(x.IsT0);
            Assert.True(x.IsT1);
            Assert.False(x.IsT2);
            Assert.False(x.IsT3);
            Assert.Equal(x.AsT1, (false));
            x = 'a';
            Assert.True(x.HasValue);
            Assert.False(x.IsT0);
            Assert.False(x.IsT1);
            Assert.True(x.IsT2);
            Assert.False(x.IsT3);
            Assert.Equal(x.AsT2, ('a'));
            x = "seven";
            Assert.True(x.HasValue);
            Assert.False(x.IsT0);
            Assert.False(x.IsT1);
            Assert.False(x.IsT2);
            Assert.True(x.IsT3);
            Assert.Equal(x.AsT3, ("seven"));
        }

    }
}
