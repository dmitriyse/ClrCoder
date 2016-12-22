// <copyright file="Interval.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
namespace ClrCoder.Logic
{
    using System;

    using JetBrains.Annotations;

    using Newtonsoft.Json;

    /// <summary>
    /// Abstract inteval on some comparable dimension.
    /// </summary>
    /// <typeparam name="T">Dimension type.</typeparam>
    [PublicAPI]
    public struct Interval<T> : IEquatable<Interval<T>>
        where T : struct, IComparable<T>, IEquatable<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Interval{T}"/> struct.
        /// </summary>
        /// <param name="start">Interval start (inclusive).</param>
        /// <param name="endExclusive">Interval end (exclusive).</param>
        [JsonConstructor]
        public Interval(T? start, T? endExclusive)
            : this()
        {
            if (start != null && endExclusive != null
                && start.Value.CompareTo(endExclusive.Value) > 0)
            {
                throw new ArgumentException("Interval start cannot be grater than interval end.");
            }

            Start = start;
            EndExclusive = endExclusive;
        }

        /// <summary>
        /// The <c>interval</c> has zero length.
        /// </summary>
        [JsonIgnore]
        public bool IsEmpty => Start != null && Start.Equals(EndExclusive);

        /// <summary>
        /// Interval start (inclusive).
        /// </summary>
        public T? Start { get; }

        /// <summary>
        /// Interval end (exclusive).
        /// </summary>
        public T? EndExclusive { get; }

        /// <summary>
        /// Determines if a value belongs to the interval.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns><see langword="true"/>, if the specified value belongs to the interval, otherwise <see langword="false"/>.</returns>
        public bool Contains(T value)
        {
            // ReSharper disable ReplaceWithSingleAssignment.True
            var result = true;
            if (Start != null && value.CompareTo(Start.Value) < 0)
            {
                result = false;
            }

            // ReSharper restore ReplaceWithSingleAssignment.True
            if (EndExclusive != null && value.CompareTo(EndExclusive.Value) >= 0)
            {
                result = false;
            }

            return result;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                return (Start.GetHashCode() * 397) ^ EndExclusive.GetHashCode();
            }
        }

        /// <inheritdoc/>
        public bool Equals(Interval<T> other)
        {
            return Start.Equals(other.Start) && EndExclusive.Equals(other.EndExclusive);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is Interval<T>)
            {
                return Equals((Interval<T>)obj);
            }

            return false;
        }
    }
}