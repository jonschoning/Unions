using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Unions
{
    /// <summary>
    /// Represents a single value of which is either a "success" or an accumultaed list of "failures". Create by direct assignment from TSuccess or TFailure (the correct ctor is implicity called); Use Match() to get the value back out.  
    /// </summary>
    /// <typeparam name="TSuccess">The type indicating success</typeparam>
    /// <typeparam name="TFailure">The type indicating failure</typeparam>
    [DebuggerStepThrough]
    [Serializable]
    public struct AccResult<TSuccess, TFailure>  : IEquatable<AccResult<TSuccess, TFailure>>, IEquatable<TSuccess>, ISerializable {
        private readonly object successValue;
        private readonly List<TFailure> failureValues;
        private readonly int? index;

        /// <summary>
        /// Represents a single value of which is either a "success" or an accumultaed list of "failures".
        /// </summary>
        /// <param name="value"></param>
        public AccResult(TSuccess value) {
            this.successValue = value;
            this.failureValues = null;
            if (value != null) index = 0;
            else index = null;
        }
        /// <summary>
        /// Represents a single value of which is either a "success" or an accumultaed list of "failures".
        /// </summary>
        /// <param name="value"></param>
        public AccResult(TFailure value) {
            failureValues = new List<TFailure>();
            this.failureValues.Add(value);
            this.successValue = null;
            if (value != null) index = 1;
            else index = null;
        }
        /// <summary>
        /// Represents a single value of which is either a "success" or an accumultaed list of "failures".
        /// </summary>
        /// <param name="value"></param>
        public AccResult(IEnumerable<TFailure> value) {
            failureValues = value.ToList();
            this.successValue = null;
            if (value != null) index = 1;
            else index = null;
        }

        /// <summary>
        /// used for serialization
        /// </summary>
        /// <param name="info"></param>
        /// <param name="text"></param>
        public AccResult(SerializationInfo info, StreamingContext text) : this()
        {
            index = (int?)info.GetValue("i", typeof(int?));
            if (index == 0) { successValue = info.GetValue("v", typeof(TSuccess)); }
            else if (index == 1) { failureValues = (List<TFailure>)info.GetValue("v", typeof(List<TFailure>)); }
            else if (index == null) { }
            else throw new SerializationException("invalid index");
        }
        /// <summary>
        /// used for serialization
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if(index == 1) {
                info.AddValue("v", failureValues);
            } else {
                info.AddValue("v", successValue);
            }
            info.AddValue("i", index);
        }

        /// <summary>
        /// Get the value (untyped)
        /// </summary>
        
        public object Value { get { return index == 1 ? (object)failureValues : successValue; } }
        /// <summary>
        /// true if value != null
        /// </summary>
        
        public bool HasValue { get { return index != null; } }

        void CheckIndex() {
            if (index == null) throw new InvalidOperationException("value not initialized");
        }
        void CheckIndex(int index) {
            CheckIndex();
            if (index != this.index) {
                throw new InvalidOperationException("Cannot return as T" + index + " . value is T" + this.index);
            }
        }


        /// <summary>
        /// returns True if the value is a TSuccess
        /// </summary>
        
        public bool IsSuccess { get { return successValue != null && index == 0; } }

        /// <summary>
        /// casts the value to a TSuccess
        /// </summary>
        
        public TSuccess AsSuccess { get { CheckIndex(0); return (TSuccess)successValue;  } }

        /// <summary>
        /// construct from a TSuccess
        /// </summary>
        /// <param name="t"></param>
        public static implicit operator AccResult<TSuccess, TFailure>(TSuccess t) {
            return new AccResult<TSuccess, TFailure>(t);
        }

        /// <summary>
        /// returns True if the value is a TFailure
        /// </summary>
        
        public bool IsFailure { get { return failureValues != null && index == 1; } }

        /// <summary>
        /// cast the value to a TFaliure
        /// </summary>
        
        public List<TFailure> AsFailure { get { CheckIndex(1); return failureValues; } }

        /// <summary>
        /// construct from a TFailure
        /// </summary>
        /// <param name="t"></param>
        public static implicit operator AccResult<TSuccess, TFailure>(TFailure t) { return new AccResult<TSuccess, TFailure>(t); }
        public static implicit operator AccResult<TSuccess, TFailure>(List<TFailure> t) { return new AccResult<TSuccess, TFailure>(t); }
        public static implicit operator AccResult<TSuccess, TFailure>(TFailure [] t) { return new AccResult<TSuccess, TFailure>(t); }
        public static implicit operator AccResult<TSuccess, TFailure>(HashSet<TFailure> t) { return new AccResult<TSuccess, TFailure>(t); }
        public static implicit operator AccResult<TSuccess, TFailure>(SortedSet<TFailure> t) { return new AccResult<TSuccess, TFailure>(t); }

        /// <summary>
        /// run the action Equal corresponds to the type of the actual value. throws if value == null
        /// </summary>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        public void Match(Action<TSuccess> f0, Action<IEnumerable<TFailure>> f1) {
            CheckIndex();

            if (this.IsFailure && f1 != null) { f1(this.AsFailure); return; }
            if (this.IsSuccess && f0 != null) { f0(this.AsSuccess); return; }

            throw new InvalidOperationException();
        }


        /// <summary>
        /// run the fuction Equal corresponds to the type of the actual value.. throws if value == null
        /// </summary>
        /// <typeparam name="TAccResult"></typeparam>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <returns></returns>
        public TAccResult Match<TAccResult>(Func<TSuccess, TAccResult> f0, Func<IEnumerable<TFailure>, TAccResult> f1) {
            CheckIndex();

            if (this.IsFailure && f1 != null) return f1(this.AsFailure);
            if (this.IsSuccess && f0 != null) return f0(this.AsSuccess);

            throw new InvalidOperationException();
        }

        /// <summary>
        /// optionally run the fuction Equal corresponds to the type of the actual value.  throws if no function matches.
        /// </summary>
        public TAccResult MatchSome<TAccResult>(Func<TSuccess, TAccResult> f0 = null, Func<IEnumerable<TFailure>, TAccResult> f1 = null, Func<TAccResult> otherwise = null) {
            CheckIndex();

            if (this.IsFailure && f1 != null) return f1(this.AsFailure);
            if (this.IsSuccess && f0 != null) return f0(this.AsSuccess);

            if (otherwise != null) return otherwise();
            throw new InvalidOperationException();
        }

        /// <summary>
        /// if the value is a success, transform it with the suplied function. throws if value == null
        /// </summary>
        public AccResult<TSuccess2, TFailure> MapSuccess<TSuccess2>(Func<TSuccess, TSuccess2> f1) {
            CheckIndex();

            if (this.IsFailure) return this.AsFailure;
            if (this.IsSuccess && f1 != null) return f1(this.AsSuccess);

            throw new InvalidOperationException();
        }

        /// <summary>
        /// alias for MapSuccess. if the value is a success, transform it with the suplied function. throws if value == null
        /// </summary>
        public AccResult<TSuccess2, TFailure> Map<TSuccess2>(Func<TSuccess, TSuccess2> f1) {
            return MapSuccess<TSuccess2>(f1);
        }

        /// <summary>
        /// if the value is a failure, transform it with the suplied function. throws if value == null
        /// </summary>
        public AccResult<TSuccess, TFailure2> MapFailure<TFailure2>(Func<IEnumerable<TFailure>, TFailure2> f1) {
            CheckIndex();

            if (this.IsFailure && f1 != null) return f1(this.AsFailure);
            if (this.IsSuccess) return this.AsSuccess;

            throw new InvalidOperationException();
        }

        /// <summary>
        /// transform both sides of the value . throws if value == null
        /// </summary>
        public AccResult<TSuccess2, TFailure2> MapBoth<TSuccess2,TFailure2>(Func<TSuccess, TSuccess2> f0, Func<IEnumerable<TFailure>, TFailure2> f1) {
            CheckIndex();

            if (this.IsFailure && f1 != null) return f1(this.AsFailure);
            if (this.IsSuccess && f0 != null) return f0(this.AsSuccess);

            throw new InvalidOperationException();
        }

        /// <summary>
        /// if either are failures, result is combined failure, else run the func on the success value 
        /// </summary>
        /// <typeparam name="TSuccess2"></typeparam>
        /// <param name="e1"></param>
        /// <param name="e2"></param>
        /// <returns></returns>
        public static AccResult<TSuccess2, TFailure> Apply<TSuccess2>(AccResult<Func<TSuccess, TSuccess2>, TFailure> e1, AccResult<TSuccess, TFailure> e2) {
            e1.CheckIndex();
            e2.CheckIndex();

            if (e1.IsFailure && e2.IsFailure) return e1.AsFailure.Concat(e2.AsFailure).ToList();
            else if (e1.IsFailure && e2.IsSuccess) return e1.AsFailure;
            else if (e1.IsSuccess && e2.IsFailure) return e2.AsFailure;

            return e1.AsSuccess(e2.AsSuccess);
        }

        /// <summary>
        /// GetHashCode of the underlying value
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            unchecked {
                return ((index == 1 ? failureValues.GetHashCode() : (index == 0 ? successValue.GetHashCode() : 0)) * 397) ^ index.GetValueOrDefault();
            }
        }

        /// <summary>
        /// ToString on the underlying value
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return ((index == 1 ? failureValues.ToString() : (index == 0 ? successValue.ToString() : ""))); }



        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(TSuccess other) {
            if (index == null) return false;
            if (IsSuccess) return EqualityComparer<TSuccess>.Default.Equals(AsSuccess, other);
            return false;
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(List<TFailure> other) {
            if (index == null) return false;
            if (IsFailure) return EqualityComparer<List<TFailure>>.Default.Equals(AsFailure, other);
            return false;
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(AccResult<TSuccess, TFailure> other) {
            if (index == null && other.index == null) return true;
            if (index == null || other.index == null) return false;
            if (IsSuccess) return other.Equals(AsSuccess);
            if (IsFailure) return other.Equals(AsFailure);
            return false;
        }

        /// <summary>
        /// ==
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(AccResult<TSuccess, TFailure> a, AccResult<TSuccess, TFailure> b) {
            return a.Equals(b);
        }

        /// <summary>
        /// !=
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(AccResult<TSuccess, TFailure> a, AccResult<TSuccess, TFailure> b) {
            return !a.Equals(b);
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) {
            return obj is AccResult<TSuccess, TFailure> ? this.Equals((AccResult<TSuccess, TFailure>)obj) : false;
        }

        /// <summary>
        /// convert to a an Either
        /// </summary>
        /// <returns></returns>
        public Either<TSuccess, IEnumerable<TFailure>> ToEither() {
            return Match(t0 => t0, t1 => new Either<TSuccess, IEnumerable<TFailure>>(t1));
        }
    }

    [DebuggerStepThrough]
    public static class AccResultOps
    {
        /// <summary>
        /// if either are failures, result is combined failure; else run the func else run the func on the success value 
        /// </summary>
        public static AccResult<TSuccess2, TFailure> Apply<TSuccess,TSuccess2,TFailure>(this AccResult<Func<TSuccess, TSuccess2>, TFailure> e1, AccResult<TSuccess, TFailure> e2) {
            return AccResult<TSuccess,TFailure>.Apply(e1, e2);
        }

        /// <summary>
        /// lift a 2-argument function over 2 AccResults.  (If all AccResults are IsSuccess, apply the function to the success values; else results in Failure)
        /// </summary>
        public static AccResult<C,Z> LiftA2<A,B,C,Z>(Func<A,B,C> f, AccResult<A,Z> a, AccResult<B,Z> b) {
            return a.Map(f.Curry()).Apply(b);
        }

        /// <summary>
        /// lift a 3-argument function over 3 AccResults.  (If all AccResults are IsSuccess, apply the function to the success values; else results in Failure)
        /// </summary>
        public static AccResult<D,Z> LiftA3<A,B,C,D,Z>(Func<A,B,C,D> f, AccResult<A,Z> a, AccResult<B,Z> b, AccResult<C,Z> c) {
            return a.Map(f.Curry()).Apply(b).Apply(c);
        }

        /// <summary>
        /// lift a 4-argument function over 4 AccResults.  (If all AccResults are IsSuccess, apply the function to the success values; else results in Failure)
        /// </summary>
        public static AccResult<E,Z> LiftA4<A,B,C,D,E,Z>(Func<A,B,C,D,E> f, AccResult<A,Z> a, AccResult<B,Z> b, AccResult<C,Z> c, AccResult<D,Z> d) {
            return a.Map(f.Curry()).Apply(b).Apply(c).Apply(d);
        }

        /// <summary>
        /// Collect the results from each AccResult in the input list into a single AccResult
        /// </summary>
        public static AccResult<IEnumerable<TSuccess>,TFailure> SequenceA<TSuccess,TFailure>(this IEnumerable<AccResult<TSuccess,TFailure>> xs) {
            List<TFailure> f = new List<TFailure>();
            List<TSuccess> s = new List<TSuccess>();
            foreach(var x in xs) {
                x.Match(s.Add, f.AddRange);
            }
            if (f.Any()) {
                return f;
            } else {
                return s;
            }
        }

        /// <summary>
        /// Collect the results from each AccResult projection in the input list into a single AccResult
        /// </summary>
        public static AccResult<IEnumerable<TSuccess>,TFailure> Traverse<A,TSuccess,TFailure>(Func<A, AccResult<TSuccess,TFailure>> f, IEnumerable<A> xs) {
            return xs.Select(f).SequenceA();
        }

    }

}
