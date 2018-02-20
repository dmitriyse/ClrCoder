// <copyright file="ConcurrentEnumerator.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Concurrent enumeration adapter over IEnumerable.
    /// </summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    /// <typeparam name="TEnumerator">The type of the enumerator.</typeparam>
    public struct ConcurrentEnumerator<T, TEnumerator> : IConcurrentEnumerator<T>
        where TEnumerator : IEnumerator<T>
    {
        private readonly object _syncRoot;
        private readonly TEnumerator _enumerator;

        private bool _enumerationFinished;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentEnumerator{T,TEnumerator}"/> struct. 
        /// </summary>
        /// <param name="enumerator">The underlying non concurrent enumerator.</param>
        public ConcurrentEnumerator(TEnumerator enumerator)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (enumerator == null)
            {
                throw new ArgumentNullException(nameof(enumerator));
            }

            _enumerator = enumerator;
            _syncRoot = new object();
            _enumerationFinished = false;
        }

        /// <inheritdoc/>
        public bool TryGetNext(out T item)
        {
            lock (_syncRoot)
            {
                if (_enumerationFinished)
                {
                    item = default;
                    return false;
                }

                if (!_enumerator.MoveNext())
                {
                    item = default;
                    _enumerationFinished = true;
                    return false;
                }

                item = _enumerator.Current;
                return true;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _enumerator.Dispose();
        }
    }
}