// <copyright file="MultiInterval.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Logic
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;

    /// <summary>
    /// Multi-part interval.
    /// </summary>
    /// <remarks>
    /// Parts collection is always sorted by <see cref="Interval{T}.Start"/>.<br/>
    /// Parts collection does not contains interval intersections.
    /// </remarks>
    /// <typeparam name="T">Type of scale.</typeparam>
    [PublicAPI]
    public class MultiInterval<T> : IReadOnlyCollection<Interval<T>>
        where T : struct, IComparable<T>, IEquatable<T>
    {
        private static readonly IComparer<T?> EndBoundaryComparer = NullBiggestComparer<T?>.Default;

        private static readonly IComparer<T?> StartBoundaryComparer = Comparer<T?>.Default;

        private readonly List<Interval<T>> _parts = new List<Interval<T>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiInterval{T}"/> class.
        /// </summary>
        /// <param name="intervals">
        /// A parts to build-up multi-interval.<br/>
        /// Parts collection is always sorted by <see cref="Interval{T}.Start"/>.<br/>
        /// Parts collection does not contains interval intersections.
        /// </param>
        public MultiInterval(IEnumerable<Interval<T>> intervals)
        {
            _parts.AddRange(intervals);
            VerifyIsValid();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiInterval{T}"/> class.
        /// </summary>
        public MultiInterval()
        {
        }

        /// <inheritdoc/>
        public int Count => _parts.Count;

        /// <inheritdoc/>
        public IEnumerator<Interval<T>> GetEnumerator()
        {
            return _parts.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Clones current multi-part interval.
        /// </summary>
        /// <returns>Multi-part interval clone.</returns>
        public MultiInterval<T> Clone()
        {
            var result = new MultiInterval<T>();
            result._parts.AddRange(_parts);
            return result;
        }

        /// <summary>
        /// Converts to interval that covers all parts of the multiinterval.
        /// </summary>
        /// <returns>An interval that covers all parts of the multiinterval.</returns>
        public Interval<T> ToInterval()
        {
            if (_parts.Any())
            {
                return new Interval<T>(_parts.First().Start, _parts.Last().EndExclusive);
            }
            else
            {
                return new Interval<T>(default(T), default(T));
            }
        }

        /// <summary>
        /// Union current multi-part <c>interval</c> with single <c>interval</c>. <br/>
        /// Uniun result is written to current multi-part <c>interval</c>.
        /// </summary>
        /// <param name="interval">An <c>interval</c> to union with.</param>
        public void Union(Interval<T> interval)
        {
            if (interval.IsEmpty)
            {
                return;
            }

            int i;
            for (i = 0; i < _parts.Count - 1; i++)
            {
                if (StartBoundaryComparer.Compare(interval.Start, _parts[i + 1].Start) < 0)
                {
                    break;
                }
            }

            // Checking that interval.Start is inside start part.
            int newIntervalStartIndex;
            T? newIntervalStart;

            if (_parts.Count != 0)
            {
                if (interval.Start == null)
                {
                    newIntervalStart = null;
                    newIntervalStartIndex = i;
                }
                else
                {
                    if (EndBoundaryComparer.Compare(interval.Start, _parts[i].EndExclusive) <= 0)
                    {
                        if (StartBoundaryComparer.Compare(interval.Start, _parts[i].Start) < 0)
                        {
                            // interval.Start is before start part.
                            newIntervalStart = interval.Start;
                        }
                        else
                        {
                            newIntervalStart = _parts[i].Start;
                        }

                        newIntervalStartIndex = i;
                    }
                    else
                    {
                        // interval.Start is after start part.
                        newIntervalStart = interval.Start;
                        newIntervalStartIndex = i + 1;
                    }
                }
            }
            else
            {
                newIntervalStart = interval.Start;
                newIntervalStartIndex = 0;
            }

            int nextPartIndex = _parts.Count;
            for (; i < _parts.Count; i++)
            {
                if (EndBoundaryComparer.Compare(interval.EndExclusive, _parts[i].Start) < 0)
                {
                    nextPartIndex = i;
                    break;
                }
            }

            T? newIntervalEndExclusive;
            if (newIntervalStartIndex != nextPartIndex)
            {
                if (EndBoundaryComparer.Compare(interval.EndExclusive, _parts[nextPartIndex - 1].EndExclusive) > 0)
                {
                    newIntervalEndExclusive = interval.EndExclusive;
                }
                else
                {
                    newIntervalEndExclusive = _parts[nextPartIndex - 1].EndExclusive;
                }

                // Updating part
                _parts[newIntervalStartIndex] = new Interval<T>(newIntervalStart, newIntervalEndExclusive);

                // Remove overlapped parts.
                _parts.RemoveRange(newIntervalStartIndex + 1, nextPartIndex - newIntervalStartIndex - 1);
            }
            else
            {
                // This case is possible when new period must be inserted between two others.
                // Include case when new interval must be placed at the end of parts list.
                _parts.Insert(newIntervalStartIndex, interval);
            }

#if DEBUG
            VerifyIsValid();
#endif
        }

        /// <summary>
        /// Searches interval index, that contains specified <c>point</c>.
        /// </summary>
        /// <param name="startIndex">Start index to start search.</param>
        /// <param name="point">Point to seach interval for.</param>
        /// <returns>Index of the interval that contains specified <c>point</c>.</returns>
        private int FindIntervalIndex(int startIndex, T? point)
        {
            Comparer<T?> comparer = Comparer<T?>.Default;
            for (int i = startIndex; i < _parts.Count; i++)
            {
                if (comparer.Compare(_parts[i].Start, point) >= 0)
                {
                    if (_parts[i].EndExclusive == null || comparer.Compare(point, _parts[i].EndExclusive) < 0)
                    {
                        return i;
                    }

                    break;
                }
            }

            return -1;
        }

        private void VerifyIsValid()
        {
            Comparer<T?> comparer = Comparer<T?>.Default;
            for (var i = 0; i < _parts.Count; i++)
            {
                if (_parts[i].IsEmpty)
                {
                    throw new ArgumentException("MultiInterval part can not be empty.");
                }

                if (i != 0 && _parts[i].Start == null)
                {
                    throw new ArgumentException(
                        "Only the first multi-interval part can have opened starting boundary.");
                }

                if (i != _parts.Count - 1)
                {
                    if (_parts[i].EndExclusive == null)
                    {
                        throw new ArgumentException(
                            "Only the last multi-interval part can have opened ending boundary.");
                    }

                    // Checking parts order.
                    if (comparer.Compare(_parts[i].Start, _parts[i + 1].Start) >= 0)
                    {
                        throw new ArgumentException(
                            "Multi-interval parts are not sorted.");
                    }

                    // Checking parts are not intersected.
                    if (comparer.Compare(_parts[i].EndExclusive, _parts[i + 1].Start) > 0)
                    {
                        throw new ArgumentException("Multi-interval parts have intersections.");
                    }
                }
            }
        }
    }
}