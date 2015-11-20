using System;
using System.Collections.Generic;

namespace ClrCoder.System
{
    /// <summary>
    /// Valued alternative to the <see cref="Tuple{T1}"/> class. <br/>
    /// TODO: Optimize performance. 
    /// </summary>
    /// <typeparam name="T1">First item type.</typeparam>
    public struct ValuedTuple<T1> : IEquatable<ValuedTuple<T1>>
    {
        /// <summary>
        /// Item1 value.
        /// </summary>
        public readonly T1 Item1;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValuedTuple{T1, T2}"/> struct.
        /// </summary>
        /// <param name="item1">Item1 value.</param>
        public ValuedTuple(T1 item1)
        {
            Item1 = item1;
        }

        /// <summary>
        /// Equality <see langword="operator"/>.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>true, if operands are equal, false otherwise.</returns>
        public static bool operator ==(ValuedTuple<T1> left, ValuedTuple<T1> right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Inequality <see langword="operator"/>.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns>true, if operands are different, false otherwise.</returns>
        public static bool operator !=(ValuedTuple<T1> left, ValuedTuple<T1> right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Cast <see langword="operator"/>.
        /// </summary>
        /// <param name="tuple">Tuple to cast.</param>
        /// <returns>Item1 value.</returns>
        public static implicit operator T1(ValuedTuple<T1> tuple)
        {
            return tuple.Item1;
        }

        /// <summary>
        /// Cast <see langword="operator"/>.
        /// </summary>
        /// <param name="item1">Value to cast to tuple.</param>
        /// <returns>Tuple with the specified value.</returns>
        public static implicit operator ValuedTuple<T1>(T1 item1)
        {
            return new ValuedTuple<T1>(item1);
        }

        /// <inheritdoc/>
        public bool Equals(ValuedTuple<T1> other)
        {
            return EqualityComparer<T1>.Default.Equals(Item1, other.Item1);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is ValuedTuple<T1> && Equals((ValuedTuple<T1>)obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return EqualityComparer<T1>.Default.GetHashCode(Item1);
        }
    }
}