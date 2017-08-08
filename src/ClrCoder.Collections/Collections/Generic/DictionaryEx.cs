// <copyright file="DictionaryEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Collections.Generic
{
    using JetBrains.Annotations;

    /// <inheritdoc cref="Dictionary{TKey,TValue}"/>
    public class DictionaryEx<TKey, TValue> : Dictionary<TKey, TValue>, IDictionaryEx<TKey, TValue>
    {
        /// <inheritdoc/>
        public DictionaryEx()
        {
        }

        /// <inheritdoc/>
        public DictionaryEx([CanBeNull] IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
        }

        /// <inheritdoc/>
        public DictionaryEx(int capacity)
            : base(capacity)
        {
        }

        /// <inheritdoc/>
        public DictionaryEx(IDictionary<TKey, TValue> dictionary)
            : base(dictionary)
        {
        }

        /// <inheritdoc/>
        public DictionaryEx(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
            : base(dictionary, comparer)
        {
        }
    }
}