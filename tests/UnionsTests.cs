using NUnit.Framework;
using System.Collections.Generic;
using System;

namespace Unions.Tests
{
    [TestFixture]
    public class UnionsTests
    {
        [Test]
        public void Unions_Maybe_HasValues() {

            // value types
            var x = default(Optional<int>);
            Assert.IsTrue(x.IsNone);
            Assert.IsFalse(x.IsSomeT);
            Assert.IsFalse(x.IsSomeNull);
            x = None.Value;
            Assert.IsTrue(x.IsNone);
            Assert.IsFalse(x.IsSomeT);
            Assert.IsFalse(x.IsSomeNull);
            x = 0;
            Assert.IsTrue(x.IsSomeT);
            Assert.IsFalse(x.IsNone);
            Assert.IsFalse(x.IsSomeNull);
            x = 7;
            Assert.IsTrue(x.IsSomeT);
            Assert.IsFalse(x.IsNone);
            Assert.IsFalse(x.IsSomeNull);

            // reference types
            var y = default(Optional<string>);
            Assert.IsTrue(y.IsNone);
            Assert.IsFalse(y.IsSomeT);
            Assert.IsFalse(y.IsSomeNull);
            y = null;                  // implicit convertion to None
            Assert.IsTrue(y.IsNone);
            Assert.IsFalse(y.IsSomeT);
            Assert.IsFalse(y.IsSomeNull);
            y = Optional<string>.FromSome("7");
            Assert.IsFalse(y.IsNone);
            Assert.IsTrue(y.IsSomeT);
            Assert.IsFalse(y.IsSomeNull);   
            y = new Some<string>(null); // explicit Some(null)
            Assert.IsFalse(y.IsNone);
            Assert.IsTrue(y.IsSomeT);
            Assert.IsTrue(y.IsSomeNull);   
            y = Optional<string>.FromSome(null); // explicit Some(null)
            Assert.IsFalse(y.IsNone);
            Assert.IsTrue(y.IsSomeT);
            Assert.IsTrue(y.IsSomeNull);   
            y = y.ToNoneIfNull();
            Assert.IsTrue(y.IsNone);
            Assert.IsFalse(y.IsSomeT);
            Assert.IsFalse(y.IsSomeNull);
            y = None.Value;
            Assert.IsTrue(y.IsNone);
            Assert.IsFalse(y.IsSomeT);
            Assert.IsFalse(y.IsSomeNull);
            y = string.Empty;
            Assert.IsTrue(y.IsSomeT);
            Assert.IsFalse(y.IsNone);
            Assert.IsFalse(y.IsSomeNull);
            y = "seven";
            Assert.IsTrue(y.IsSomeT);
            Assert.IsFalse(y.IsNone);
            Assert.IsFalse(y.IsSomeNull);
        }
        [Test]
        public void Unions_Result__HasValues() {
            var x = default(Result<int, string>);
            Assert.IsFalse(x.HasValue);
            Assert.IsFalse(x.IsSuccess);
            Assert.IsFalse(x.IsFailure);
            x = null;
            Assert.IsFalse(x.HasValue);
            Assert.IsFalse(x.IsSuccess);
            Assert.IsFalse(x.IsFailure);
            x = 0;
            Assert.IsTrue(x.HasValue);
            Assert.IsTrue(x.IsSuccess);
            Assert.IsFalse(x.IsFailure);
            Assert.That(x.AsSuccess, Is.EqualTo(0));
            x = 7;
            Assert.IsTrue(x.HasValue);
            Assert.IsTrue(x.IsSuccess);
            Assert.IsFalse(x.IsFailure);
            Assert.That(x.AsSuccess, Is.EqualTo(7));
            x = "seven";
            Assert.IsTrue(x.HasValue);
            Assert.IsFalse(x.IsSuccess);
            Assert.IsTrue(x.IsFailure);
            Assert.That(x.AsFailure, Is.EqualTo("seven"));
        }


        [Test]
        public void Unions_AccResult__HasValues() {
            var x = default(AccResult<int, string>);
            Assert.IsFalse(x.HasValue);
            Assert.IsFalse(x.IsSuccess);
            Assert.IsFalse(x.IsFailure);
            x = 0;
            Assert.IsTrue(x.HasValue);
            Assert.IsTrue(x.IsSuccess);
            Assert.IsFalse(x.IsFailure);
            Assert.That(x.AsSuccess, Is.EqualTo(0));
            x = 7;
            Assert.IsTrue(x.HasValue);
            Assert.IsTrue(x.IsSuccess);
            Assert.IsFalse(x.IsFailure);
            Assert.That(x.AsSuccess, Is.EqualTo(7));
            x = "seven";
            Assert.IsTrue(x.HasValue);
            Assert.IsFalse(x.IsSuccess);
            Assert.IsTrue(x.IsFailure);
            Assert.That(x.AsFailure, Is.EquivalentTo((new List<string>() { "seven" })));

            var isEven = new AccResult<Func<int, bool>, string>(_ => _ % 2 == 0);
            var ret = isEven.Apply(x);
            Assert.IsTrue(ret.HasValue);
            Assert.IsFalse(ret.IsSuccess);
            Assert.IsTrue(ret.IsFailure);
            Assert.That(ret.AsFailure, Is.EquivalentTo((new List<string>() { "seven" })));

            x = 8;
            ret = isEven.Apply(x);
            Assert.IsTrue(ret.HasValue);
            Assert.IsTrue(ret.IsSuccess);
            Assert.IsFalse(ret.IsFailure);
            Assert.That(ret.AsSuccess, Is.EqualTo(true));

            var hasFailure = new AccResult<Func<int, bool>, string>("failure");
            ret = hasFailure.Apply(x);
            Assert.IsTrue(ret.HasValue);
            Assert.IsFalse(ret.IsSuccess);
            Assert.IsTrue(ret.IsFailure);
            Assert.That(ret.AsFailure, Is.EquivalentTo((new List<string>() { "failure" })));

            x = "bothfailure";
            ret = hasFailure.Apply(x);
            Assert.IsTrue(ret.HasValue);
            Assert.IsFalse(ret.IsSuccess);
            Assert.IsTrue(ret.IsFailure);
            Assert.That(ret.AsFailure, Is.EquivalentTo((new List<string>() { "failure", "bothfailure" })));

            var ret2 = hasFailure.Apply(x);
            Assert.That(ret.AsFailure, Is.EquivalentTo(ret2.AsFailure));

            // apply (success)
            var a = new AccResult<int,string>(3);
            var b = new AccResult<int,string>(4);
            var c = new AccResult<int,string>(5);
            var applyResult = AccResultOps.LiftA3(Tuple.Create, a, b, c);
            var expected = Tuple.Create(3, 4, 5);
            Assert.That(applyResult.AsSuccess, Is.EqualTo(expected));


            // apply (failure)
            var a2 = new AccResult<int,string>(3);
            var b2 = new AccResult<int,string>("failure 1");
            var c2 = new AccResult<int,string>("failure 2");
            var applyResult2 = AccResultOps.LiftA3(Tuple.Create, a2, b2, c2);
            var expected2 = new List<string>() { "failure 1", "failure 2" };
            Assert.That(applyResult2.AsFailure, Is.EquivalentTo(expected2));

            // sequence (success)
            var accList = new AccResult<int, string>[] { 3, 4, 5, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,  3, 4, 5, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            var sequenceResult = accList.SequenceA();
            var successExpected = new int[] { 3, 4, 5, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,  3, 4, 5, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            Assert.That(sequenceResult.AsSuccess, Is.EquivalentTo(successExpected));

            // sequence (fail)
            var accList2 = new AccResult<int, string>[] { 4, 5, 1, 1, 1, 13, "fail 1", 3, 1, "fail 2", 4, 5, 1, 5, "fail 3" };
            var sequenceResult2 = accList2.SequenceA();
            var failExpected2 = new string[] { "fail 1", "fail 2", "fail 3" };
            Assert.That(sequenceResult2.AsFailure, Is.EquivalentTo(failExpected2));

            // traverse (success)
            var tList = new int[] { 3, 4, 5, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,  3, 4, 5, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            var tResult = AccResultOps.Traverse<int,int,string>(p1 => p1, tList);
            Assert.That(tResult.AsSuccess, Is.EquivalentTo(tList));

            // traverse (fail)
            var t2List = new int[] { 3, 4, 5, 1, 1, 1, 1, 1, 1,  3, 6, 5, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            var t2Result = AccResultOps.Traverse<int,int,string>(p1 => {  if (p1 % 2 == 1) { return p1; } else { return "n"+p1.ToString(); }; }, t2List);
            Assert.That(t2Result.AsFailure, Is.EquivalentTo(new string[]{ "n4", "n6" }));
        }

        [Test]
        public void Unions_Either_HasValues() {
            var x = default(Either<int, string>);
            Assert.IsFalse(x.HasValue);
            Assert.IsFalse(x.IsT0);
            Assert.IsFalse(x.IsT1);
            x = null;
            Assert.IsFalse(x.HasValue);
            Assert.IsFalse(x.IsT0);
            Assert.IsFalse(x.IsT1);
            x = 0;
            Assert.IsTrue(x.HasValue);
            Assert.IsTrue(x.IsT0);
            Assert.IsFalse(x.IsT1);
            Assert.That(x.AsT0, Is.EqualTo(0));
            x = 7;
            Assert.IsTrue(x.HasValue);
            Assert.IsTrue(x.IsT0);
            Assert.IsFalse(x.IsT1);
            Assert.That(x.AsT0, Is.EqualTo(7));
            x = "seven";
            Assert.IsTrue(x.HasValue);
            Assert.IsFalse(x.IsT0);
            Assert.IsTrue(x.IsT1);
            Assert.That(x.AsT1, Is.EqualTo("seven"));
        }
        [Test]
        public void Unions_OneOf3_HasValues() {
            var x = default(OneOf<int, bool, string>);
            Assert.IsFalse(x.HasValue);
            Assert.IsFalse(x.IsT0);
            Assert.IsFalse(x.IsT1);
            Assert.IsFalse(x.IsT2);
            x = null;
            Assert.IsFalse(x.HasValue);
            Assert.IsFalse(x.IsT0);
            Assert.IsFalse(x.IsT1);
            Assert.IsFalse(x.IsT2);
            x = 0;
            Assert.IsTrue(x.HasValue);
            Assert.IsTrue(x.IsT0);
            Assert.IsFalse(x.IsT1);
            Assert.IsFalse(x.IsT2);
            Assert.That(x.AsT0, Is.EqualTo(0));
            x = 7;
            Assert.IsTrue(x.HasValue);
            Assert.IsTrue(x.IsT0);
            Assert.IsFalse(x.IsT1);
            Assert.IsFalse(x.IsT2);
            Assert.That(x.AsT0, Is.EqualTo(7));
            x = false;
            Assert.IsTrue(x.HasValue);
            Assert.IsFalse(x.IsT0);
            Assert.IsTrue(x.IsT1);
            Assert.IsFalse(x.IsT2);
            Assert.That(x.AsT1, Is.EqualTo(false));
            x = "seven";
            Assert.IsTrue(x.HasValue);
            Assert.IsFalse(x.IsT0);
            Assert.IsFalse(x.IsT1);
            Assert.IsTrue(x.IsT2);
            Assert.That(x.AsT2, Is.EqualTo("seven"));
        }
        [Test]
        public void Unions_OneOf4_HasValues() {
            var x = default(OneOf<int, bool, char, string>);
            Assert.IsFalse(x.HasValue);
            Assert.IsFalse(x.IsT0);
            Assert.IsFalse(x.IsT1);
            Assert.IsFalse(x.IsT2);
            Assert.IsFalse(x.IsT3);
            x = null;
            Assert.IsFalse(x.HasValue);
            Assert.IsFalse(x.IsT0);
            Assert.IsFalse(x.IsT1);
            Assert.IsFalse(x.IsT2);
            Assert.IsFalse(x.IsT3);
            x = 0;
            Assert.IsTrue(x.HasValue);
            Assert.IsTrue(x.IsT0);
            Assert.IsFalse(x.IsT1);
            Assert.IsFalse(x.IsT2);
            Assert.IsFalse(x.IsT3);
            Assert.That(x.AsT0, Is.EqualTo(0));
            x = 7;
            Assert.IsTrue(x.HasValue);
            Assert.IsTrue(x.IsT0);
            Assert.IsFalse(x.IsT1);
            Assert.IsFalse(x.IsT2);
            Assert.IsFalse(x.IsT3);
            Assert.That(x.AsT0, Is.EqualTo(7));
            x = false;
            Assert.IsTrue(x.HasValue);
            Assert.IsFalse(x.IsT0);
            Assert.IsTrue(x.IsT1);
            Assert.IsFalse(x.IsT2);
            Assert.IsFalse(x.IsT3);
            Assert.That(x.AsT1, Is.EqualTo(false));
            x = 'a';
            Assert.IsTrue(x.HasValue);
            Assert.IsFalse(x.IsT0);
            Assert.IsFalse(x.IsT1);
            Assert.IsTrue(x.IsT2);
            Assert.IsFalse(x.IsT3);
            Assert.That(x.AsT2, Is.EqualTo('a'));
            x = "seven";
            Assert.IsTrue(x.HasValue);
            Assert.IsFalse(x.IsT0);
            Assert.IsFalse(x.IsT1);
            Assert.IsFalse(x.IsT2);
            Assert.IsTrue(x.IsT3);
            Assert.That(x.AsT3, Is.EqualTo("seven"));
        }

    }
}
