// <copyright file="TransitiveSourceCastOperation.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Linq
{
    using Collections;
    using Collections.Generic;

    /// <summary>
    /// Extended cast linq operation that can transit source to the next operation.
    /// </summary>
    /// <typeparam name="T">Type to cast items to.</typeparam>
    internal class TransitiveSourceCastOperation<T> : IEnumerable<T>, ITransitiveSourceLinqOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransitiveSourceCastOperation{T}"/> class.
        /// </summary>
        /// <param name="source">Source enumeration.</param>
        public TransitiveSourceCastOperation(IEnumerable source)
        {
            Source = source;
        }

        /// <inheritdoc/>
        public IEnumerable Source { get; }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            return Source.Cast<T>().GetEnumerator();
        }
    }
}