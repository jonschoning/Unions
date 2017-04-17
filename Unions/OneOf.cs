using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Unions
{
    /* OneOf data structure is used to hold a single value of one of a fixed set of Types.
       "Matching" provides an alternative to abstract classes and subclasses.

    // EXAMPLE

        Rectangle rect = new Rectangle(10, 10, 10, 10);  
        OneOf<Rectangle, Circle, Prism> shape = rect;

    //You can then access the value using .IsT0 and .AsT0
        Assert.True(shape.IsT0);  
        Assert.True(shape.AsT0 is Rectangle);

    //MATCHING
    //you use the match(func f0, ... func fn) method to perform an action e.g.

        OneOf<Rectangle, Circle, Prism> shape = ...;
        shape.Match(
          rect => gfx.DrawRect(rect.X, rect.Y, rect.Width, rec.Height), 
          circle => gfx.DrawCircle(circle.X, circle.Y, circle.Radius),
          prism => ... 
        ); 

    //OR CONVERT TO ANOTHER VALUE

        OneOf<Rectangle, Circle, Prism> shape = ...;
        Int32 area = shape.Match(
          rect => rect.Width * rec.Height, 
          circle => 2 * Math.Pi * circle.Radius,
          prism => ... 
        ); 

    */

    /// <summary>
    /// Extract the value as an object
    /// </summary>
    public interface IValue {
        /// <summary>
        /// the value
        /// </summary>
        object Value { get; }
        bool HasValue { get; }
    }

    /// <summary>
    /// Represents a single value of three possibilities: it is a T0, a T1, or a T2. Create by direct assignment from T0,T1,or T2 (the correct ctor is implicity called); Use Match() to get the value back out.  
    /// </summary>
    /// <typeparam name="T0"></typeparam>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    [DebuggerStepThrough]
    [Serializable]
    public struct OneOf<T0, T1, T2> : IValue, IEquatable<OneOf<T0, T1, T2>>, ISerializable
    {
        readonly object value;
        readonly int? index;

        /// <summary>
        /// Represents a single value of three possibilities: it is a T0, a T1, or a T2.
        /// </summary>
        /// <param name="value"></param>
        public OneOf(T0 value) {
            this.value = value;
            if (value != null) index = 0;
            else index = null;
        }
        /// <summary>
        /// Represents a single value of three possibilities: it is a T0, a T1, or a T2.
        /// </summary>
        /// <param name="value"></param>
        public OneOf(T1 value) {
            this.value = value;
            if (value != null) index = 1;
            else index = null;
        }
        /// <summary>
        /// Represents a single value of three possibilities: it is a T0, a T1, or a T2.
        /// </summary>
        /// <param name="value"></param>
        public OneOf(T2 value) {
            this.value = value;
            if (value != null) index = 2;
            else index = null;
        }
        /// <summary>
        /// used for serialization
        /// </summary>
        /// <param name="info"></param>
        /// <param name="text"></param>
        public OneOf(SerializationInfo info, StreamingContext text) : this()
        {
            index = (int?)info.GetValue("i", typeof(int?));
            if (index == 0) value = info.GetValue("v", typeof(T0));
            else if (index == 1) value = info.GetValue("v", typeof(T1));
            else if (index == 2) value = info.GetValue("v", typeof(T2));
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
        /// returns True if the value is a T0
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
        public static implicit operator OneOf<T0, T1, T2>(T0 t) {
            return new OneOf<T0, T1, T2>(t);
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
        public static implicit operator OneOf<T0, T1, T2>(T1 t) {
            return new OneOf<T0, T1, T2>(t);
        }


        /// <summary>
        /// returns True if the value is a T2
        /// </summary>
        
        public bool IsT2 { get { return value != null && index == 2; } }

        /// <summary>
        /// casts the value to a T2
        /// </summary>
        
        public T2 AsT2 { get { return Get<T2>(2); } }

        /// <summary>
        /// construct from a T2
        /// </summary>
        /// <param name="t"></param>
        public static implicit operator OneOf<T0, T1, T2>(T2 t) {
            return new OneOf<T0, T1, T2>(t);
        }


        /// <summary>
        /// run the action Equal corresponds to the type of the actual value. throws if value == null
        /// </summary>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        public void Match(Action<T0> f0, Action<T1> f1, Action<T2> f2) {
            if (index == null) throw new InvalidOperationException("value not initialized");

            if (this.IsT0 && f0 != null) { f0(this.AsT0); return; }
            if (this.IsT1 && f1 != null) { f1(this.AsT1); return; }
            if (this.IsT2 && f2 != null) { f2(this.AsT2); return; }

            throw new InvalidOperationException();
        }


        /// <summary>
        /// run the fuction Equal corresponds to the type of the actual value.. throws if value == null
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        public TResult Match<TResult>(Func<T0, TResult> f0, Func<T1, TResult> f1, Func<T2, TResult> f2) {
            if (index == null) throw new InvalidOperationException("value not initialized");

            if (this.IsT0 && f0 != null) return f0(this.AsT0);
            if (this.IsT1 && f1 != null) return f1(this.AsT1);
            if (this.IsT2 && f2 != null) return f2(this.AsT2);

            throw new InvalidOperationException();
        }


        /// <summary>
        /// optionally run the fuction Equal corresponds to the type of the actual value.  throws if no function matches.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <param name="otherwise"></param>
        /// <returns></returns>
        public TResult MatchSome<TResult>(Func<T0, TResult> f0 = null, Func<T1, TResult> f1 = null, Func<T2, TResult> f2 = null, Func<TResult> otherwise = null) {
            if (index == null) throw new InvalidOperationException("value not initialized");

            if (this.IsT0 && f0 != null) return f0(this.AsT0);
            if (this.IsT1 && f1 != null) return f1(this.AsT1);
            if (this.IsT2 && f2 != null) return f2(this.AsT2);

            if (otherwise != null) return otherwise();
            throw new InvalidOperationException();
        }


        /// <summary>
        /// transform each case of the value. throws if value == null
        /// </summary>
        /// <typeparam name="TR0"></typeparam>
        /// <typeparam name="TR1"></typeparam>
        /// <typeparam name="TR2"></typeparam>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        public OneOf<TR0,TR1,TR2> Map<TR0,TR1,TR2>(Func<T0, TR0> f0, Func<T1, TR1> f1, Func<T2, TR2> f2) {
            if (index == null) throw new InvalidOperationException("value not initialized");

            if (this.IsT0 && f0 != null) return f0(this.AsT0);
            if (this.IsT1 && f1 != null) return f1(this.AsT1);
            if (this.IsT2 && f2 != null) return f2(this.AsT2);

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
        public bool Equals(T2 other) {
            if (index == null) return false;
            if (IsT2) return EqualityComparer<T2>.Default.Equals(AsT2, other);
            return false;
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(OneOf<T0, T1, T2> other) {
            if (index == null && other.index == null) return true;
            if (index == null || other.index == null) return false;
            if (IsT0) return other.Equals(AsT0);
            if (IsT1) return other.Equals(AsT1);
            if (IsT2) return other.Equals(AsT2);
            return false;
        }

        /// <summary>
        /// ==
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(OneOf<T0, T1, T2> a, OneOf<T0, T1, T2> b) {
            return a.Equals(b);
        }

        /// <summary>
        /// !=
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(OneOf<T0, T1, T2> a, OneOf<T0, T1, T2> b) {
            return !a.Equals(b);
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) {
            return obj is OneOf<T0, T1, T2> ? this.Equals((OneOf<T0, T1, T2>)obj) : false;
        }
    }

    /// <summary>
    /// Represents a single value of four possibilities: it is a T0, a T1, a T2, or a T3. Create by direct assignment from T0,T1,T2, or T3 (the correct ctor is implicity called); Use Match() to get the value back out.  
    /// </summary>
    /// <typeparam name="T0"></typeparam>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    [DebuggerStepThrough]
    [Serializable]
    public struct OneOf<T0, T1, T2, T3> : IValue, IEquatable<OneOf<T0, T1, T2, T3>>, ISerializable
    {
        readonly object value;
        readonly int? index;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="value"></param>
        public OneOf(T0 value) {
            this.value = value;
            if (value != null) index = 0;
            else index = null;
        }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="value"></param>
        public OneOf(T1 value) {
            this.value = value;
            if (value != null) index = 1;
            else index = null;
            
        }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="value"></param>
        public OneOf(T2 value) {
            this.value = value;
            if (value != null) index = 2;
            else index = null;
            
        }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="value"></param>
        public OneOf(T3 value) {
            this.value = value;
            if (value != null) index = 3;
            else index = null;
            
        }
        /// <summary>
        /// used for serialization
        /// </summary>
        /// <param name="info"></param>
        /// <param name="text"></param>
        public OneOf(SerializationInfo info, StreamingContext text) : this()
        {
            index = (int?)info.GetValue("i", typeof(int?));
            if (index == 0) value = info.GetValue("v", typeof(T0));
            else if (index == 1) value = info.GetValue("v", typeof(T1));
            else if (index == 2) value = info.GetValue("v", typeof(T2));
            else if (index == 3) value = info.GetValue("v", typeof(T3));
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


        
        public bool IsT0 { get { return value != null && index == 0; } }
        
        public T0 AsT0 { get { return Get<T0>(0); } }
        public static implicit operator OneOf<T0, T1, T2, T3>(T0 t) {
            return new OneOf<T0, T1, T2, T3>(t);
        }


        
        public bool IsT1 { get { return value != null && index == 1; } }
        
        public T1 AsT1 { get { return Get<T1>(1); } }
        public static implicit operator OneOf<T0, T1, T2, T3>(T1 t) {
            return new OneOf<T0, T1, T2, T3>(t);
        }


        
        public bool IsT2 { get { return value != null && index == 2; } }
        
        public T2 AsT2 { get { return Get<T2>(2); } }
        public static implicit operator OneOf<T0, T1, T2, T3>(T2 t) {
            return new OneOf<T0, T1, T2, T3>(t);
        }


        
        public bool IsT3 { get { return value != null && index == 3; } }
        
        public T3 AsT3 { get { return Get<T3>(3); } }
        public static implicit operator OneOf<T0, T1, T2, T3>(T3 t) {
            return new OneOf<T0, T1, T2, T3>(t);
        }


        /// <summary>
        /// throws if value == null
        /// </summary>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <param name="f3"></param>
        public void Match(Action<T0> f0, Action<T1> f1, Action<T2> f2, Action<T3> f3) {
            if (index == null) throw new InvalidOperationException("value not initialized");

            if (this.IsT0 && f0 != null) { f0(this.AsT0); return; }
            if (this.IsT1 && f1 != null) { f1(this.AsT1); return; }
            if (this.IsT2 && f2 != null) { f2(this.AsT2); return; }
            if (this.IsT3 && f3 != null) { f3(this.AsT3); return; }

            throw new InvalidOperationException();
        }


        /// <summary>
        /// throws if value == null
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <param name="f3"></param>
        /// <returns></returns>
        public TResult Match<TResult>(Func<T0, TResult> f0, Func<T1, TResult> f1, Func<T2, TResult> f2, Func<T3, TResult> f3) {
            if (index == null) throw new InvalidOperationException("value not initialized");

            if (this.IsT0 && f0 != null) return f0(this.AsT0);
            if (this.IsT1 && f1 != null) return f1(this.AsT1);
            if (this.IsT2 && f2 != null) return f2(this.AsT2);
            if (this.IsT3 && f3 != null) return f3(this.AsT3);

            throw new InvalidOperationException();
        }


        /// <summary>
        /// throws if value == null
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <param name="f3"></param>
        /// <param name="otherwise"></param>
        /// <returns></returns>
        public TResult MatchSome<TResult>(Func<T0, TResult> f0 = null, Func<T1, TResult> f1 = null, Func<T2, TResult> f2 = null, Func<T3, TResult> f3 = null, Func<TResult> otherwise = null) {
            if (index == null) throw new InvalidOperationException("value not initialized");

            if (this.IsT0 && f0 != null) return f0(this.AsT0);
            if (this.IsT1 && f1 != null) return f1(this.AsT1);
            if (this.IsT2 && f2 != null) return f2(this.AsT2);
            if (this.IsT3 && f3 != null) return f3(this.AsT3);

            if (otherwise != null) return otherwise();
            throw new InvalidOperationException();
        }

        /// <summary>
        /// throws if value == null
        /// </summary>
        /// <typeparam name="TR0"></typeparam>
        /// <typeparam name="TR1"></typeparam>
        /// <typeparam name="TR2"></typeparam>
        /// <typeparam name="TR3"></typeparam>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <param name="f3"></param>
        /// <returns></returns>
        public OneOf<TR0,TR1,TR2,TR3> Map<TR0,TR1,TR2,TR3>(Func<T0, TR0> f0, Func<T1, TR1> f1, Func<T2, TR2> f2, Func<T3, TR3> f3) {
            if (index == null) throw new InvalidOperationException("value not initialized");

            if (this.IsT0 && f0 != null) return f0(this.AsT0);
            if (this.IsT1 && f1 != null) return f1(this.AsT1);
            if (this.IsT2 && f2 != null) return f2(this.AsT2);
            if (this.IsT3 && f3 != null) return f3(this.AsT3);

            throw new InvalidOperationException();
        }

        public override int GetHashCode() {
            unchecked {
                return ((index != null ? value.GetHashCode() : 0) * 397) ^ index.GetValueOrDefault();
            }
        }
        public override string ToString() { return value != null ? value.ToString() : string.Empty; }

        public bool Equals(T0 other) {
            if (index == null) return false;
            if (IsT0) return EqualityComparer<T0>.Default.Equals(AsT0, other);
            return false;
        }
        public bool Equals(T1 other) {
            if (index == null) return false;
            if (IsT1) return EqualityComparer<T1>.Default.Equals(AsT1, other);
            return false;
        }
        public bool Equals(T2 other) {
            if (index == null) return false;
            if (IsT2) return EqualityComparer<T2>.Default.Equals(AsT2, other);
            return false;
        }
        public bool Equals(T3 other) {
            if (index == null) return false;
            if (IsT3) return EqualityComparer<T3>.Default.Equals(AsT3, other);
            return false;
        }

        public bool Equals(OneOf<T0, T1, T2, T3> other) {
            if (index == null && other.index == null) return true;
            if (index == null || other.index == null) return false;
            if (IsT0) return other.Equals(AsT0);
            if (IsT1) return other.Equals(AsT1);
            if (IsT2) return other.Equals(AsT2);
            if (IsT3) return other.Equals(AsT3);
            return false;
        }

        public static bool operator ==(OneOf<T0, T1, T2, T3> a, OneOf<T0, T1, T2, T3> b) {
            return a.Equals(b);
        }
        public static bool operator !=(OneOf<T0, T1, T2, T3> a, OneOf<T0, T1, T2, T3> b) {
            return !a.Equals(b);
        }
        public override bool Equals(object obj) {
            return obj is OneOf<T0, T1, T2, T3> ? this.Equals((OneOf<T0, T1, T2, T3>)obj) : false;
        }
    }

    /// <summary>
    /// Represents a single value of five possibilities: it is a T0, a T1, a T2, a T3, or a T4. Create by direct assignment from T0,T1,T2,T3, or T4 (the correct ctor is implicity called); Use Match() to get the value back out.  
    /// </summary>
    /// <typeparam name="T0"></typeparam>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    [DebuggerStepThrough]
    [Serializable]
    public struct OneOf<T0, T1, T2, T3, T4> : IValue, IEquatable<OneOf<T0, T1, T2, T3, T4>>, ISerializable
    {
        readonly object value;
        readonly int? index;

        private OneOf(object value, int index) {
            this.value = value;
            this.index = index;
        }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="value"></param>
        public OneOf(T0 value) {
            this.value = value;
            if (value != null) index = 0;
            else index = null;
        }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="value"></param>
        public OneOf(T1 value) {
            this.value = value;
            if (value != null) index = 1;
            else index = null;
            
        }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="value"></param>
        public OneOf(T2 value) {
            this.value = value;
            if (value != null) index = 2;
            else index = null;
            
        }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="value"></param>
        public OneOf(T3 value) {
            this.value = value;
            if (value != null) index = 3;
            else index = null;
            
        }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="value"></param>
        public OneOf(T4 value) {
            this.value = value;
            if (value != null) index = 4;
            else index = null;
            
        }

        /// <summary>
        /// used for serialization
        /// </summary>
        /// <param name="info"></param>
        /// <param name="text"></param>
        public OneOf(SerializationInfo info, StreamingContext text) : this()
        {
            index = (int?)info.GetValue("i", typeof(int?));
            if (index == 0) value = info.GetValue("v", typeof(T0));
            else if (index == 1) value = info.GetValue("v", typeof(T1));
            else if (index == 2) value = info.GetValue("v", typeof(T2));
            else if (index == 3) value = info.GetValue("v", typeof(T3));
            else if (index == 4) value = info.GetValue("v", typeof(T4));
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


        
        public bool IsT0 { get { return value != null && index == 0; } }
        
        public T0 AsT0 { get { return Get<T0>(0); } }
        public static implicit operator OneOf<T0, T1, T2, T3, T4>(T0 t) {
            return new OneOf<T0, T1, T2, T3, T4>(t);
        }


        
        public bool IsT1 { get { return value != null && index == 1; } }
        
        public T1 AsT1 { get { return Get<T1>(1); } }
        public static implicit operator OneOf<T0, T1, T2, T3, T4>(T1 t) {
            return new OneOf<T0, T1, T2, T3, T4>(t);
        }


        
        public bool IsT2 { get { return value != null && index == 2; } }
        
        public T2 AsT2 { get { return Get<T2>(2); } }
        public static implicit operator OneOf<T0, T1, T2, T3, T4>(T2 t) {
            return new OneOf<T0, T1, T2, T3, T4>(t);
        }


        
        public bool IsT3 { get { return value != null && index == 3; } }
        
        public T3 AsT3 { get { return Get<T3>(3); } }
        public static implicit operator OneOf<T0, T1, T2, T3, T4>(T3 t) {
            return new OneOf<T0, T1, T2, T3, T4>(t);
        }


        
        public bool IsT4 { get { return value != null && index == 4; } }
        
        public T4 AsT4 { get { return Get<T4>(4); } }
        public static implicit operator OneOf<T0, T1, T2, T3, T4>(T4 t) {
            return new OneOf<T0, T1, T2, T3, T4>(t);
        }


        /// <summary>
        /// throws if value == null
        /// </summary>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <param name="f3"></param>
        /// <param name="f4"></param>
        public void Match(Action<T0> f0, Action<T1> f1, Action<T2> f2, Action<T3> f3, Action<T4> f4) {
            if (index == null) throw new InvalidOperationException("value not initialized");

            if (this.IsT0 && f0 != null) { f0(this.AsT0); return; }
            if (this.IsT1 && f1 != null) { f1(this.AsT1); return; }
            if (this.IsT2 && f2 != null) { f2(this.AsT2); return; }
            if (this.IsT3 && f3 != null) { f3(this.AsT3); return; }
            if (this.IsT4 && f4 != null) { f4(this.AsT4); return; }

            throw new InvalidOperationException();
        }


        /// <summary>
        /// . throws if value == null
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <param name="f3"></param>
        /// <param name="f4"></param>
        /// <returns></returns>
        public TResult Match<TResult>(Func<T0, TResult> f0, Func<T1, TResult> f1, Func<T2, TResult> f2, Func<T3, TResult> f3, Func<T4, TResult> f4) {
            if (index == null) throw new InvalidOperationException("value not initialized");

            if (this.IsT0 && f0 != null) return f0(this.AsT0);
            if (this.IsT1 && f1 != null) return f1(this.AsT1);
            if (this.IsT2 && f2 != null) return f2(this.AsT2);
            if (this.IsT3 && f3 != null) return f3(this.AsT3);
            if (this.IsT4 && f4 != null) return f4(this.AsT4);

            throw new InvalidOperationException();
        }


        /// <summary>
        /// . throws if value == null
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <param name="f3"></param>
        /// <param name="f4"></param>
        /// <param name="otherwise"></param>
        /// <returns></returns>
        public TResult MatchSome<TResult>(Func<T0, TResult> f0 = null, Func<T1, TResult> f1 = null, Func<T2, TResult> f2 = null, Func<T3, TResult> f3 = null, Func<T4, TResult> f4 = null, Func<TResult> otherwise = null) {
            if (index == null) throw new InvalidOperationException("value not initialized");

            if (this.IsT0 && f0 != null) return f0(this.AsT0);
            if (this.IsT1 && f1 != null) return f1(this.AsT1);
            if (this.IsT2 && f2 != null) return f2(this.AsT2);
            if (this.IsT3 && f3 != null) return f3(this.AsT3);
            if (this.IsT4 && f4 != null) return f4(this.AsT4);

            if (otherwise != null) return otherwise();
            throw new InvalidOperationException();
        }

        /// <summary>
        /// . throws if value == null
        /// </summary>
        /// <typeparam name="TR0"></typeparam>
        /// <typeparam name="TR1"></typeparam>
        /// <typeparam name="TR2"></typeparam>
        /// <typeparam name="TR3"></typeparam>
        /// <typeparam name="TR4"></typeparam>
        /// <param name="f0"></param>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <param name="f3"></param>
        /// <param name="f4"></param>
        /// <returns></returns>
        public OneOf<TR0,TR1,TR2,TR3,TR4> Map<TR0,TR1,TR2,TR3,TR4>(Func<T0, TR0> f0, Func<T1, TR1> f1, Func<T2, TR2> f2, Func<T3, TR3> f3, Func<T4, TR4> f4) {
            if (index == null) throw new InvalidOperationException("value not initialized");

            if (this.IsT0 && f0 != null) return f0(this.AsT0);
            if (this.IsT1 && f1 != null) return f1(this.AsT1);
            if (this.IsT2 && f2 != null) return f2(this.AsT2);
            if (this.IsT3 && f3 != null) return f3(this.AsT3);
            if (this.IsT4 && f4 != null) return f4(this.AsT4);

            throw new InvalidOperationException();
        }

        public override int GetHashCode() {
            unchecked {
                return ((index != null ? value.GetHashCode() : 0) * 397) ^ index.GetValueOrDefault();
            }
        }

        public override string ToString() { return value != null ? value.ToString() : string.Empty; }

        public bool Equals(T0 other) {
            if (index == null) return false;
            if (IsT0) return EqualityComparer<T0>.Default.Equals(AsT0, other);
            return false;
        }
        public bool Equals(T1 other) {
            if (index == null) return false;
            if (IsT1) return EqualityComparer<T1>.Default.Equals(AsT1, other);
            return false;
        }
        public bool Equals(T2 other) {
            if (index == null) return false;
            if (IsT2) return EqualityComparer<T2>.Default.Equals(AsT2, other);
            return false;
        }
        public bool Equals(T3 other) {
            if (index == null) return false;
            if (IsT3) return EqualityComparer<T3>.Default.Equals(AsT3, other);
            return false;
        }
        public bool Equals(T4 other) {
            if (index == null) return false;
            if (IsT4) return EqualityComparer<T4>.Default.Equals(AsT4, other);
            return false;
        }

        public bool Equals(OneOf<T0, T1, T2, T3, T4> other) {
            if (index == null && other.index == null) return true;
            if (index == null || other.index == null) return false;
            if (IsT0) return other.Equals(AsT0);
            if (IsT1) return other.Equals(AsT1);
            if (IsT2) return other.Equals(AsT2);
            if (IsT4) return other.Equals(AsT4);
            return false;
        }

        public static bool operator ==(OneOf<T0, T1, T2, T3, T4> a, OneOf<T0, T1, T2, T3, T4> b) {
            return a.Equals(b);
        }
        public static bool operator !=(OneOf<T0, T1, T2, T3, T4> a, OneOf<T0, T1, T2, T3, T4> b) {
            return !a.Equals(b);
        }
        public override bool Equals(object obj) {
            return obj is OneOf<T0, T1, T2, T3, T4> ? this.Equals((OneOf<T0, T1, T2, T3, T4>)obj) : false;
        }
    }

}
