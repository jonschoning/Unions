using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Unions
{
    /// <summary>
    /// This type represents a single value of two possibilities: it is either T0 or T1.  Create by direct assignment from T0 or T1 (the correct ctor is implicity called); Use Match() to get the value back out.
    /// </summary>
    /// <typeparam name="T0"></typeparam>
    /// <typeparam name="T1"></typeparam>
    [DebuggerStepThrough]
    [Serializable]
    public struct Either<T0, T1> : IValue, IEquatable<Either<T0, T1>>, ISerializable
    {
        private readonly object value;
        private readonly int? index;

        /// <summary>
        /// Represents a single value of two possibilities: it is either T0 or T1.
        /// </summary>
        /// <param name="value"></param>
        public Either(T0 value) {
            this.value = value;
            if (value != null) index = 0;
            else index = null;
        }
        /// <summary>
        /// Represents a single value of two possibilities: it is either T0 or T1.
        /// </summary>
        /// <param name="value"></param>
        public Either(T1 value) {
            this.value = value;
            if (value != null) index = 1;
            else index = null;
        }

        /// <summary>
        /// used for serialization
        /// </summary>
        /// <param name="info"></param>
        /// <param name="text"></param>
        public Either(SerializationInfo info, StreamingContext text) : this()
        {
            index = (int?)info.GetValue("i", typeof(int?));
            if (index == 0) value = info.GetValue("v", typeof(T0));
            else if (index == 1) value = info.GetValue("v", typeof(T1));
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

        /// <summary>
        /// returns True if the index is a T0
        /// </summary>
        
        public bool IsT0 { get { return value != null && index == 0; } }

        /// <summary>
        /// casts the value to a T0
        /// </summary>
        
        public T0 AsT0 { get { return Get<T0>(0); } }

        /// <summary>
        /// construct from a T0
        /// </summary>
        /// <param name="t"></param>
        public static implicit operator Either<T0, T1>(T0 t) {
            return new Either<T0,T1>(t);
        }


        /// <summary>
        /// returns True if the value is a T1
        /// </summary>
        
        public bool IsT1 { get { return value != null && index == 1; } }

        /// <summary>
        /// casts the value to a T1
        /// </summary>
        
        public T1 AsT1 { get { return Get<T1>(1); } }

        /// <summary>
        /// construct from a T1
        /// </summary>
        /// <param name="t"></param>
        public static implicit operator Either<T0, T1>(T1 t) {
            return new Either<T0,T1>(t);
        }


        /// <summary>
        /// run the action that corresponds to the type of the actual value. throws if value == null
        /// </summary>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        public void Match(Action<T0> f0, Action<T1> f1) {
            if (index == null) throw new InvalidOperationException("value not initialized");

            if (this.IsT0 && f0 != null) { f0(this.AsT0); return; }
            if (this.IsT1 && f1 != null) { f1(this.AsT1); return; }

            throw new InvalidOperationException();
        }


        /// <summary>
        /// run the fuction that corresponds to the type of the actual value. throws if value == null.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <returns></returns>
        public TResult Match<TResult>(Func<T0, TResult> f0, Func<T1, TResult> f1) {
            if (index == null) throw new InvalidOperationException("value not initialized");

            if (this.IsT0 && f0 != null) return f0(this.AsT0);
            if (this.IsT1 && f1 != null) return f1(this.AsT1);

            throw new InvalidOperationException();
        }


        /// <summary>
        /// optionally run the fuction that corresponds to the type of the actual value.  throws if no function matches. throws if value == null
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <param name="otherwise"></param>
        /// <returns></returns>
        public TResult MatchSome<TResult>(Func<T0, TResult> f0 = null, Func<T1, TResult> f1 = null, Func<TResult> otherwise = null) {
            if (index == null) throw new InvalidOperationException("value not initialized");

            if (this.IsT0 && f0 != null) return f0(this.AsT0);
            if (this.IsT1 && f1 != null) return f1(this.AsT1);

            if (otherwise != null) return otherwise();
            throw new InvalidOperationException();
        }

        /// <summary>
        /// if the value is a T0, transform it with the suplied function. throws if value == null
        /// </summary>
        /// <typeparam name="T0Result"></typeparam>
        /// <param name="f0"></param>
        /// <returns></returns>
        public Either<T0Result, T1> MapT0<T0Result>(Func<T0, T0Result> f0) {
            if (index == null) throw new InvalidOperationException("value not initialized");

            if (this.IsT1) return this.AsT1;
            if (this.IsT0 && f0 != null) return f0(this.AsT0);

            throw new InvalidOperationException();
        }

        /// <summary>
        /// if the value is a T1, transform it with the suplied function. throws if value == null
        /// </summary>
        /// <typeparam name="T1Result"></typeparam>
        /// <param name="f1"></param>
        /// <returns></returns>
        public Either<T0, T1Result> MapT1<T1Result>(Func<T1, T1Result> f1) {
            if (index == null) throw new InvalidOperationException("value not initialized");

            if (this.IsT0) return this.AsT0;
            if (this.IsT1 && f1 != null) return f1(this.AsT1);

            throw new InvalidOperationException();
        }

        /// <summary>
        /// transform both sides of the value. throws if value == null 
        /// </summary>
        /// <typeparam name="TR0"></typeparam>
        /// <typeparam name="TR1"></typeparam>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <returns></returns>
        public Either<TR0,TR1> MapBoth<TR0,TR1>(Func<T0, TR0> f0, Func<T1, TR1> f1) {
            if (index == null) throw new InvalidOperationException("value not initialized");

            if (this.IsT0 && f0 != null) return f0(this.AsT0);
            if (this.IsT1 && f1 != null) return f1(this.AsT1);

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
        public bool Equals(T0 other) {
            if (index == null) return false;
            if (IsT0) return EqualityComparer<T0>.Default.Equals(AsT0, other);
            return false;
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(T1 other) {
            if (index == null) return false;
            if (IsT1) return EqualityComparer<T1>.Default.Equals(AsT1, other);
            return false;
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Either<T0, T1> other) {
            if (index == null && other.index == null) return true;
            if (index == null || other.index == null) return false;
            if (IsT0) return other.Equals(AsT0);
            if (IsT1) return other.Equals(AsT1);
            return false;
        }

        /// <summary>
        /// ==
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Either<T0, T1> a, Either<T0, T1> b) {
            return a.Equals(b);
        }

        /// <summary>
        /// !=
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(Either<T0, T1> a, Either<T0, T1> b) {
            return !a.Equals(b);
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) {
            return obj is Either<T0, T1> ? this.Equals((Either<T0, T1>)obj) : false;
        }
    }

    [DebuggerStepThrough]
    public static class EitherOps
    {
        /// <summary>
        /// convert to a Result
        /// </summary>
        /// <returns></returns>
        public static Result<TSuccess, TFailure> ToResult<TSuccess, TFailure>(this Either<TSuccess, TFailure> x) {
            return x.Match<Result<TSuccess, TFailure>>(t0 => t0, t1 => t1);
        }

        /// <summary>
        /// convert to AccResult
        /// </summary>
        public static AccResult<TSuccess, TFailure> ToAccResult<TSuccess, TFailure>(this Either<TSuccess, TFailure> x) {
            return x.Match<AccResult<TSuccess, TFailure>>(t0 => t0, t1 => t1);
        }
    }
}
