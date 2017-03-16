using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Unions
{
    /// <summary>
    /// Represents a single value of which is either a "success" or a "failure". Create by direct assignment from TSuccess or TFailure (the correct ctor is implicity called); Use Match() to get the value back out.  
    /// </summary>
    /// <typeparam name="TSuccess">The type indicating success</typeparam>
    /// <typeparam name="TFailure">The type indicating failure</typeparam>
    [DebuggerStepThrough]
    [Serializable]
    public struct Result<TSuccess, TFailure>  : IEquatable<Result<TSuccess, TFailure>>, IEquatable<TSuccess>, ISerializable {
        private readonly object value;
        private readonly int? index;

        /// <summary>
        /// Represents a single value of which is either a "success" or a "failure".
        /// </summary>
        /// <param name="value"></param>
        public Result(TSuccess value) {
            this.value = value;
            if (value != null) index = 0;
            else index = null;
        }
        /// <summary>
        /// Represents a single value of which is either a "success" or a "failure".
        /// </summary>
        /// <param name="value"></param>
        public Result(TFailure value) {
            this.value = value;
            if (value != null) index = 1;
            else index = null;
        }
        /// <summary>
        /// used for serialization
        /// </summary>
        /// <param name="info"></param>
        /// <param name="text"></param>
        public Result(SerializationInfo info, StreamingContext text) : this()
        {
            index = (int?)info.GetValue("i", typeof(int?));
            if (index == 0) value = info.GetValue("v", typeof(TSuccess));
            else if (index == 1) value = info.GetValue("v", typeof(TFailure));
            else if (index == null) { }
            else throw new SerializationException("invalid index");
        }
        /// <summary>
        /// used for serialization
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("v", value);
            info.AddValue("i", index);
        }

        /// <summary>
        /// Get the value (untyped)
        /// </summary>
        
        public object Value { get { return value; } }
        /// <summary>
        /// true if value != null
        /// </summary>
        
        public bool HasValue { get { return value != null && index != null; } }

        T Get<T>(int index) {
            if (this.index == null) throw new InvalidOperationException("value not initialized");
            if (index != this.index) {
                throw new InvalidOperationException("Cannot return as T" + index + " . value is T" + this.index);
            }
            return (T)value;
        }

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
        
        public bool IsSuccess { get { return value != null && index == 0; } }

        /// <summary>
        /// casts the value to a TSuccess
        /// </summary>
        
        public TSuccess AsSuccess { get { return Get<TSuccess>(0); } }

        /// <summary>
        /// construct from a TSuccess
        /// </summary>
        /// <param name="t"></param>
        public static implicit operator Result<TSuccess, TFailure>(TSuccess t) {
            return new Result<TSuccess, TFailure>(t);
        }

        /// <summary>
        /// returns True if the value is a TFailure
        /// </summary>
        
        public bool IsFailure { get { return value != null && index == 1; } }

        /// <summary>
        /// cast the value to a TFaliure
        /// </summary>
        
        public TFailure AsFailure { get { return Get<TFailure>(1); } }

        /// <summary>
        /// construct from a TFailure
        /// </summary>
        /// <param name="t"></param>
        public static implicit operator Result<TSuccess, TFailure>(TFailure t) {
            return new Result<TSuccess, TFailure>(t);
        }


        /// <summary>
        /// run the action that corresponds to the type of the actual value. throws if value == null
        /// </summary>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        public void Match(Action<TSuccess> f0, Action<TFailure> f1) {
            if (index == null) throw new InvalidOperationException("value not initialized");

            if (this.IsFailure && f1 != null) { f1(this.AsFailure); return; }
            if (this.IsSuccess && f0 != null) { f0(this.AsSuccess); return; }

            throw new InvalidOperationException();
        }


        /// <summary>
        /// run the fuction that corresponds to the type of the actual value.. throws if value == null
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <returns></returns>
        public TResult Match<TResult>(Func<TSuccess, TResult> f0, Func<TFailure, TResult> f1) {
            if (index == null) throw new InvalidOperationException("value not initialized");

            if (this.IsFailure && f1 != null) return f1(this.AsFailure);
            if (this.IsSuccess && f0 != null) return f0(this.AsSuccess);

            throw new InvalidOperationException();
        }

        /// <summary>
        /// optionally run the fuction that corresponds to the type of the actual value.  throws if no function matches.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <param name="otherwise">always executes if no other functions matches</param>
        /// <returns></returns>
        public TResult MatchSome<TResult>(Func<TSuccess, TResult> f0 = null, Func<TFailure, TResult> f1 = null, Func<TResult> otherwise = null) {
            if (index == null) throw new InvalidOperationException("value not initialized");

            if (this.IsFailure && f1 != null) return f1(this.AsFailure);
            if (this.IsSuccess && f0 != null) return f0(this.AsSuccess);

            if (otherwise != null) return otherwise();
            throw new InvalidOperationException();
        }

        /// <summary>
        /// if the value is a success, transform it with the suplied function. throws if value == null
        /// </summary>
        /// <typeparam name="TSuccess2"></typeparam>
        /// <param name="f1"></param>
        /// <returns></returns>
        public Result<TSuccess2, TFailure> MapSuccess<TSuccess2>(Func<TSuccess, TSuccess2> f1) {
            if (index == null) throw new InvalidOperationException("value not initialized");

            if (this.IsFailure) return this.AsFailure;
            if (this.IsSuccess && f1 != null) return f1(this.AsSuccess);

            throw new InvalidOperationException();
        }


        /// <summary>
        /// alias for MapSuccess. if the value is a success, transform it with the suplied function. throws if value == null
        /// </summary>
        public Result<TSuccess2, TFailure> Map<TSuccess2>(Func<TSuccess, TSuccess2> f1) {
            return MapSuccess<TSuccess2>(f1);
        }

        /// <summary>
        /// if the value is a failure, transform it with the suplied function. throws if value == null
        /// </summary>
        /// <typeparam name="TFailure2"></typeparam>
        /// <param name="f1"></param>
        /// <returns></returns>
        public Result<TSuccess, TFailure2> MapFailure<TFailure2>(Func<TFailure, TFailure2> f1) {
            if (index == null) throw new InvalidOperationException("value not initialized");

            if (this.IsFailure && f1 != null) return f1(this.AsFailure);
            if (this.IsSuccess) return this.AsSuccess;

            throw new InvalidOperationException();
        }

        /// <summary>
        /// transform both sides of the value . throws if value == null
        /// </summary>
        /// <typeparam name="TSuccess2"></typeparam>
        /// <typeparam name="TFailure2"></typeparam>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <returns></returns>
        public Result<TSuccess2, TFailure2> MapBoth<TSuccess2,TFailure2>(Func<TSuccess, TSuccess2> f0, Func<TFailure, TFailure2> f1) {
            if (index == null) throw new InvalidOperationException("value not initialized");

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
        public static Result<TSuccess2, TFailure> Apply<TSuccess2>(Result<Func<TSuccess, TSuccess2>, TFailure> e1, Result<TSuccess, TFailure> e2) {
            e1.CheckIndex();
            e2.CheckIndex();

            if (e1.IsFailure && e2.IsFailure) return e1.AsFailure;
            else if (e1.IsFailure && e2.IsSuccess) return e1.AsFailure;
            else if (e1.IsSuccess && e2.IsFailure) return e2.AsFailure;

            return e1.AsSuccess(e2.AsSuccess);
        }

        /// <summary>
        /// if the value is a success, run the supplied function to produce another Unions.Result. throws if value == null
        /// </summary>
        /// <typeparam name="TSuccess2"></typeparam>
        /// <param name="f1"></param>
        /// <returns></returns>
        public Result<TSuccess2, TFailure> BindSuccess<TSuccess2>(Func<TSuccess, Result<TSuccess2, TFailure>> f1) {
            if (index == null) throw new InvalidOperationException("value not initialized");
            if (this.IsFailure) return this.AsFailure;
            if (this.IsSuccess && f1 != null) return f1(this.AsSuccess);

            throw new InvalidOperationException();
        }

        /// <summary>
        /// GetHashCode of the underlying value
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            unchecked {
                return ((index != null ? value.GetHashCode() : 0) * 397) ^ index.GetValueOrDefault();
            }
        }

        /// <summary>
        /// ToString on the underlying value
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return index != null ? value.ToString() : string.Empty; }



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
        public bool Equals(TFailure other) {
            if (index == null) return false;
            if (IsFailure) return EqualityComparer<TFailure>.Default.Equals(AsFailure, other);
            return false;
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Result<TSuccess, TFailure> other) {
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
        public static bool operator ==(Result<TSuccess, TFailure> a, Result<TSuccess, TFailure> b) {
            return a.Equals(b);
        }

        /// <summary>
        /// !=
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(Result<TSuccess, TFailure> a, Result<TSuccess, TFailure> b) {
            return !a.Equals(b);
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) {
            return obj is Result<TSuccess, TFailure> ? this.Equals((Result<TSuccess, TFailure>)obj) : false;
        }

    }


    [DebuggerStepThrough]
    public static class ResultOps
    {
        /// <summary>
        /// convert to a an Either
        /// </summary>
        /// <returns></returns>
        public static Either<TSuccess, TFailure> ToEither<TSuccess, TFailure>(this Result<TSuccess, TFailure> x) {
            return x.Match<Either<TSuccess, TFailure>>(t0 => t0, t1 => t1);
        }

        /// <summary>
        /// convert to AccResult
        /// </summary>
        public static AccResult<TSuccess, TFailure> ToAccResult<TSuccess, TFailure>(this Result<TSuccess, TFailure> x) {
            return x.Match<AccResult<TSuccess, TFailure>>(t0 => t0, t1 => t1);
        }

        /// <summary>
        /// if either are failures, result is combined failure; else run the func else run the func on the success value 
        /// </summary>
        public static Result<TSuccess2, TFailure> Apply<TSuccess,TSuccess2,TFailure>(this Result<Func<TSuccess, TSuccess2>, TFailure> e1, Result<TSuccess, TFailure> e2) {
            return Result<TSuccess,TFailure>.Apply(e1, e2);
        }

        /// <summary>
        /// lift a 2-argument function over 2 Results.  (If all Results are IsSuccess, apply the function to the success values; else results in Failure)
        /// </summary>
        public static Result<C,Z> LiftA2<A,B,C,Z>(Func<A,B,C> f, Result<A,Z> a, Result<B,Z> b) {
            return a.Map(f.Curry()).Apply(b);
        }

        /// <summary>
        /// lift a 3-argument function over 3 Results.  (If all Results are IsSuccess, apply the function to the success values; else results in Failure)
        /// </summary>
        public static Result<D,Z> LiftA3<A,B,C,D,Z>(Func<A,B,C,D> f, Result<A,Z> a, Result<B,Z> b, Result<C,Z> c) {
            return a.Map(f.Curry()).Apply(b).Apply(c);
        }

        /// <summary>
        /// lift a 4-argument function over 4 Results.  (If all Results are IsSuccess, apply the function to the success values; else results in Failure)
        /// </summary>
        public static Result<E,Z> LiftA4<A,B,C,D,E,Z>(Func<A,B,C,D,E> f, Result<A,Z> a, Result<B,Z> b, Result<C,Z> c, Result<D,Z> d) {
            return a.Map(f.Curry()).Apply(b).Apply(c).Apply(d);
        }

        /// <summary>
        /// Collect the results from each Result in the input list into a single Result
        /// </summary>
        public static Result<IEnumerable<TSuccess>,TFailure> SequenceA<TSuccess,TFailure>(this IEnumerable<Result<TSuccess,TFailure>> xs) {
            List<TSuccess> s = new List<TSuccess>();
            foreach(var x in xs) {
                if (x.IsFailure) return x.AsFailure;
                s.Add(x.AsSuccess);
            }
            return s;
        }

        /// <summary>
        /// Collect the results from each Result projection in the input list into a single Result
        /// </summary>
        public static Result<IEnumerable<TSuccess>,TFailure> Traverse<A,TSuccess,TFailure>(Func<A, Result<TSuccess,TFailure>> f, IEnumerable<A> xs) {
            return xs.Select(f).SequenceA();
        }
    }
}
