// <copyright file="ListConcurrentEnumerator.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Threading;

    /// <summary>
    /// The concurrent enumerator over a list.
    /// </summary>
    /// <typeparam name="T">The type of the list elements.</typeparam>
    /// <typeparam name="TList">The type of the list to enumerate.</typeparam>
    public struct ListConcurrentEnumerator<T, TList> : IConcurrentEnumerator<T>
        where TList : IReadOnlyList<T>
    {
        private readonly TList _list;

        private readonly int _itemsCount;

        private int _nextItemIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListConcurrentEnumerator{T,TList}"/> struct.
        /// </summary>
        /// <param name="list">The list to enumerate.</param>
        public ListConcurrentEnumerator(TList list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            _list = list;
            _itemsCount = _list.Count;
            _nextItemIndex = 0;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetNext()
        {
            int itemIndex = Interlocked.Increment(ref _nextItemIndex) - 1;
            if (itemIndex >= _itemsCount)
            {
                throw new InvalidOperationException("No more items left.");
            }

            return _list[itemIndex];
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            // Do nothing.
        }
    }
}