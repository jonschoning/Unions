using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Unions
{
    /// <summary>
    /// represents the absence of a value
    /// </summary>
    [DebuggerStepThrough]
    public struct None : IEquatable<None> {
        private static None _value = new None();
        public static None Value { get { return _value; } }
        public bool Equals(None other) { return true; }
    }

    /// <summary>
    /// represents the presence of a type of value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerStepThrough]
    public struct Some<T> {
        public readonly T _value;
        public Some(T value) { _value = value; }
        public T Value { get { return _value; } }
    }

    /// <summary>
    /// Represents a single value of which is either a "T" or a "None". Create by direct assignment from T, null, Some&lt;T&gt;, or None (the correct ctor is implicity called); Use Match() to get the value back out.  
    ///
    /// coercion rules:
    ///
    /// Optional(x)  /*when x != null*/  -> param Converts to: Some(x) 
    /// Optional(null)                   -> param Converts to: None
    /// Optional(new Some<string>(null)) -> param Converts to: Some(null)
    ///
    /// </summary>
    /// <typeparam name="T">The type indicating T</typeparam>
    [DebuggerStepThrough]
    [Serializable]
    public struct Optional<T>  : IEquatable<Optional<T>>, IEquatable<T>, ISerializable {
        private readonly T value;
        private readonly bool isSome;

        /// <summary>
        /// Represents a single value of which is either a "T" or a "None". contruct from a T.  Null values are coerced into a None (null is removed). 
        /// </summary>
        /// <param name="value"></param>
        public Optional(T value) {
            this.value = value;
            this.isSome = value != null;
        }

        /// <summary>
        /// Represents a single value of which is either a "T" or a "None". contruct from a Some. Null values inside Some are *not* coerced into a None.
        /// </summary>
        /// <param name="value"></param>
        public Optional(Some<T> value) {
            this.value = value.Value;
            this.isSome = true;
        }

        /// <summary>
        /// create an optional filled with None
        /// </summary>
        /// <param name="value"></param>
        public Optional(None value) {
            this.value = default(T);
            this.isSome = false;
        }

        /// <summary>
        /// create an optional filled with None
        /// </summary>
        /// <returns></returns>
        
        public static Optional<T> FromNone { get { return None.Value; } }

        /// <summary>
        /// Construct from a Some applied to value. Null values are *not* coerced into a None.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Optional<T> FromSome(T value) { return new Some<T>(value); }

        /// <summary>
        /// if Value is null, return a new None optional, otherwise returns this instance
        /// </summary>
        /// <returns></returns>
        public Optional<T> ToNoneIfNull() {
            if (this.value == null) return None.Value;
            return this;
        }

        /// <summary>
        /// used for serialization
        /// </summary>
        /// <param name="info"></param>
        /// <param name="text"></param>
        public Optional(SerializationInfo info, StreamingContext text) : this()
        {

            value = (T)info.GetValue("v", typeof(T));
            isSome = info.GetInt32("i") == 1;
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
            info.AddValue("i", isSome ? 1 : 0);
        }

        /// <summary>
        /// Get the value (untyped)
        /// </summary>
        public object Value { get { return value; } }

        /// <summary>
        /// returns True if the value is a T
        /// </summary>
        
        public bool IsSomeT { get { return isSome; } }

        /// <summary>
        /// casts the value to a T
        /// </summary>
        public T AsSomeT {
            get {
                if (IsNone) throw new InvalidOperationException("Cannot return a T when the state is None");
                return value;
           }
        }

        /// <summary>
        /// contruct from a T.  If T is null, Value becomes a None 
        /// </summary>
        /// <param name="t"></param>
        public static implicit operator Optional<T>(T t) {
            return new Optional<T>(t);
        }

        /// <summary>
        /// contruct from a Some(T).
        /// </summary>
        /// <param name="t"></param>
        public static implicit operator Optional<T>(Some<T> t) {
            return new Optional<T>(t);
        }

        /// <summary>
        /// returns True if the value is a None
        /// </summary>
        
        public bool IsNone { get { return !isSome; } }

        /// <summary>
        /// returns True if the value is null
        /// </summary>
        public bool IsSomeNull { get { return isSome && value == null; } }

        /// <summary>
        /// Return a None if the value is not a T (IsNone)
        /// </summary>
        public None AsNone { get {
                if (!IsNone) throw new InvalidOperationException("Cannot return a None when the value is a T");
                return None.Value;
           }
        }

        /// <summary>
        /// construct from a TNone
        /// </summary>
        /// <param name="t"></param>
        public static implicit operator Optional<T>(None t) {
            return new Optional<T>(t);
        }


        /// <summary>
        /// run the action that corresponds to the type of the actual value.
        /// </summary>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        public void Match(Action<T> f0, Action f1) {
            if (this.IsNone && f1 != null) { f1(); return; }
            if (this.IsSomeT && f0 != null) { f0(this.AsSomeT); return; }

            throw new InvalidOperationException();
        }


        /// <summary>
        /// run the fuction that corresponds to the type of the actual value.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <returns></returns>
        public TResult Match<TResult>(Func<T, TResult> f0, Func<TResult> f1) {
            if (this.IsNone && f1 != null) return f1();
            if (this.IsSomeT && f0 != null) return f0(this.AsSomeT);

            throw new InvalidOperationException();
        }

        /// <summary>
        /// if the value is not None, transform it with the suplied function.
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="f1"></param>
        /// <returns></returns>
        public Optional<T2> Map<T2>(Func<T, T2> f1) {
            if (this.IsNone) return this.AsNone;
            if (this.IsSomeT && f1 != null) return f1(this.AsSomeT);

            throw new InvalidOperationException();
        }


        /// <summary>
        /// if the value is not None, run the supplied function to produce another Unions.Optional.
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="f1"></param>
        /// <returns></returns>
        public Optional<T2> Bind<T2>(Func<T, Optional<T2>> f1) {
            if (this.IsNone) return this.AsNone;
            if (this.IsSomeT && f1 != null) return f1(this.AsSomeT);

            throw new InvalidOperationException();
        }

        /// <summary>
        /// if the value is not None, apply the supplied function to produce another Unions.Optional.
        /// </summary>
        public static Optional<T2> Apply<T2>(Optional<Func<T, T2>> e1, Optional<T> e2) {
            if (e2.IsNone) return e2.AsNone;
            return e1.Map(f => f(e2.AsSomeT));
        }

        /// <summary>
        /// GetHashCode of the underlying value
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            unchecked {
                return ((value != null ? value.GetHashCode() : 0) * 397) ^ (isSome ? 1 : 0);
            }
        }

        /// <summary>
        /// ToString on the underlying value
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return isSome ? (value == null ? "null" : value.ToString()) : None.Value.ToString(); }



        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(T other) {
            if (IsSomeT) return EqualityComparer<T>.Default.Equals(AsSomeT, other);
            return false;
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(None other) {
            if (IsNone) return true;
            return false;
        }

        public bool Equals(Some<T> other) {
            if (value == null) return isSome && other.Value == null;
            if (IsSomeT) return AsSomeT.Equals(other.Value);
            return false;
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Optional<T> other) {
            if (IsNone) return other.Equals(AsNone);
            if (IsSomeT) return other.Equals(AsSomeT);
            return false;
        }

        /// <summary>
        /// ==
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Optional<T> a, Optional<T> b) {
            return a.Equals(b);
        }

        /// <summary>
        /// !=
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(Optional<T> a, Optional<T> b) {
            return !a.Equals(b);
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) {
            return obj is Optional<T> ? this.Equals((Optional<T>)obj) : false;
        }

    }


    [DebuggerStepThrough]
    public static class OptionalOps
    {
        public static Optional<bool> ToOptional(this bool? x) { if (x.HasValue) return x.Value; return None.Value; }
        public static Optional<char> ToOptional(this char? x) { if (x.HasValue) return x.Value; return None.Value; }
        public static Optional<int> ToOptional(this int? x) { if (x.HasValue) return x.Value; return None.Value; }
        public static Optional<long> ToOptional(this long? x) { if (x.HasValue) return x.Value; return None.Value; }
        public static Optional<float> ToOptional(this float? x) { if (x.HasValue) return x.Value; return None.Value; }
        public static Optional<double> ToOptional(this double? x) { if (x.HasValue) return x.Value; return None.Value; }
        public static Optional<decimal> ToOptional(this decimal? x) { if (x.HasValue) return x.Value; return None.Value; }

        /// <summary>
        /// convert to a an Either
        /// </summary>
        /// <returns></returns>
        public static Either<TSuccess, None> ToEither<TSuccess>(this Optional<TSuccess> x) {
            return x.Match<Either<TSuccess, None>>(t0 => t0, () => None.Value);
        }

        /// <summary>
        /// convert to AccResult
        /// </summary>
        public static AccResult<TSuccess, None> ToAccResult<TSuccess, TFailure>(this Optional<TSuccess> x) {
            return x.Match<AccResult<TSuccess, None>>(t0 => t0, () => None.Value);
        }

        /// <summary>
        /// if either optionals are none, result is none, else run the func on the success value 
        /// </summary>
        public static Optional<TSuccess2> Apply<TSuccess,TSuccess2>(this Optional<Func<TSuccess, TSuccess2>> e1, Optional<TSuccess> e2) {
            return Optional<TSuccess>.Apply(e1, e2);
        }
        /// <summary>
        /// lift a 2-argument function over 2 Optionals.  (If all Optionals are IsSome, apply the function to the success values; else results in None)
        /// </summary>
        public static Optional<C> LiftA2<A,B,C>(Func<A,B,C> f, Optional<A> a, Optional<B> b) {
            return a.Map(f.Curry()).Apply(b);
        }

        /// <summary>
        /// lift a 3-argument function over 3 Optionals.  (If all Optionals are IsSome, apply the function to the success values; else results in None)
        /// </summary>
        public static Optional<D> LiftA3<A,B,C,D>(Func<A,B,C,D> f, Optional<A> a, Optional<B> b, Optional<C> c) {
            return a.Map(f.Curry()).Apply(b).Apply(c);
        }

        /// <summary>
        /// lift a 4-argument function over 4 Optionals.  (If all Optionals are IsSome, apply the function to the success values; else results in None)
        /// </summary>
        public static Optional<E> LiftA4<A,B,C,D,E>(Func<A,B,C,D,E> f, Optional<A> a, Optional<B> b, Optional<C> c, Optional<D> d) {
            return a.Map(f.Curry()).Apply(b).Apply(c).Apply(d);
        }

        /// <summary>
        /// Collect the results from each Optional in the input list into a single Optional
        /// </summary>
        public static Optional<IEnumerable<T>> SequenceA<T>(this IEnumerable<Optional<T>> xs) {
            List<T> s = new List<T>();
            List<object> f = new List<object>();
            foreach(var x in xs) {
                x.Match(s.Add, () => f.Add(null));
            }
            if (f.Any()) {
                return Optional<IEnumerable<T>>.FromNone;
            } else {
                return s;
            }
        }

        /// <summary>
        /// Collect the results from each Optional projection in the input list into a single Optional
        /// </summary>
        public static Optional<IEnumerable<T>> Traverse<A,T>(Func<A, Optional<T>> f, IEnumerable<A> xs) {
            return xs.Select(f).SequenceA();
        }
    }
}
