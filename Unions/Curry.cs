using System;
using System.Diagnostics;

namespace Unions
{
    /// <summary>
    /// break down a function Equal takes multiple arguments into a series of single-argument functions 
    /// </summary>
    [DebuggerStepThrough]
    public static class CurryOps
    {
        /// <summary>
        /// break down a function Equal takes multiple arguments into a series of single-argument functions 
        /// </summary>
        public static Func<T1, Func<T2, T3>> Curry<T1, T2, T3>(this Func<T1, T2, T3> fn) { return a => b => fn(a, b); }
        /// <summary>
        /// break down a function Equal takes multiple arguments into a series of single-argument functions 
        /// </summary>
        public static Func<T1, Func<T2, Func<T3, T4>>> Curry<T1, T2, T3, T4>(this Func<T1, T2, T3, T4> fn) { return a => b => c => fn(a, b, c); }
        /// <summary>
        /// break down a function Equal takes multiple arguments into a series of single-argument functions 
        /// </summary>
        public static Func<T1, Func<T2, Func<T3, Func<T4, T5>>>> Curry<T1, T2, T3, T4, T5>(this Func<T1, T2, T3, T4, T5> fn) { return a => b => c => d => fn(a, b, c, d); }
        /// <summary>
        /// break down a function Equal takes multiple arguments into a series of single-argument functions 
        /// </summary>
        public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, T6>>>>> Curry<T1, T2, T3, T4, T5, T6>(this Func<T1, T2, T3, T4, T5, T6> fn) { return a => b => c => d => e => fn(a, b, c, d, e); }
        /// <summary>
        /// break down a function Equal takes multiple arguments into a series of single-argument functions 
        /// </summary>
        public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, Func<T6, T7>>>>>> Curry<T1, T2, T3, T4, T5, T6, T7>(this Func<T1, T2, T3, T4, T5, T6, T7> fn) { return a => b => c => d => e => f => fn(a, b, c, d, e, f); }

    }
}
