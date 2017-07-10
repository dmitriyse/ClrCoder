﻿// <copyright file="DictionaryEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Collections
{
    using System.Collections.Generic;

    using JetBrains.Annotations;

    /// <summary>
    /// Represents a collection of keys and values.
    /// </summary>
    /// <remarks>
    /// Contains implementation of additional interfaces.
    /// </remarks>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    public class DictionaryEx<TKey, TValue> : Dictionary<TKey, TValue>, IDictionaryEx<TKey, TValue>
    {
        public DictionaryEx()
        {
        }

        public DictionaryEx([CanBeNull]IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
        }

        public DictionaryEx(int capacity)
            : base(capacity)
        {
            
        }

        public DictionaryEx(IDictionary<TKey, TValue> dictionary): base(dictionary)
        {

        }

        public DictionaryEx(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer)
        {

        }
    }
}