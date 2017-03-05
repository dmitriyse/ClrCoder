// <copyright file="DictionaryExtender.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Collections.Generic
{
    using JetBrains.Annotations;

    using Linq;

    /// <summary>
    /// Extends any enumerable of keyvalue pairs to a dictionary and provides maximum possible features. Throws runtime
    /// exceptions for features that
    /// are impossible to provide.
    /// </summary>
    /// <remarks>
    /// Also class helps to emulate
    /// proposals https://github.com/dotnet/corefx/issues/16626, https://github.com/dotnet/corefx/issues/16661 on current BCL.
    /// TCollection parameter allows compiler to generate more efficient code.
    /// </remarks>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// <typeparam name="TDictionary">The type of the inner collection.</typeparam>
    /// <filterpriority>1</filterpriority>
    internal class DictionaryExtender<TKey, TValue, TDictionary> :
        CollectionExtender<KeyValuePair<TKey, TValue>, TDictionary>,
        IDictionaryEx<TKey, TValue>,
        IImmutableDictionarySlim<TKey, TValue>
        where TDictionary : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        [CanBeNull]
        private readonly IEqualityComparer<TKey> _comparer;

        private IReadOnlySet<TKey> _keys;

        private ICollectionEx<TValue> _values;

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryExtender{TKey, TValue, TDictionary}"/> class.
        /// TODO: Move to static methods allow to optimize wrappings.
        /// </summary>
        /// <param name="innerDictionary">Inner dictionary to wrap.</param>
        /// <param name="assumeImmutable">Assumes that inner collection is in immutable state.</param>
        /// <param name="assumeReadOnly">Assumes that inner collection is readonly.</param>
        /// <param name="comparer">Assumes that set uses this comparer.</param>
        public DictionaryExtender(
            TDictionary innerDictionary,
            bool assumeImmutable = false,
            bool assumeReadOnly = false,
            IEqualityComparer<TKey> comparer = null)
            : base(innerDictionary, assumeImmutable, assumeReadOnly)
        {
            _comparer = comparer;
            if (!(innerDictionary is IReadOnlyDictionary<TKey, TValue>)
                && !(innerDictionary is IDictionary<TKey, TValue>))
            {
                throw new NotSupportedException("Only IReadOnlyDictionary or IDictionary collections can be extended.");
            }
        }

        /// <inheritdoc/>
        public override bool IsReadOnly => !(InnerCollection is IDictionary<TKey, TValue>) || base.IsReadOnly;

        /// <inheritdoc/>
        public ICollection<TKey> Keys
        {
            get
            {
                if (_keys == null)
                {
                    IEnumerable<TKey> keys;
                    var readOnlyDictionary = InnerCollection as IReadOnlyDictionary<TKey, TValue>;
                    if (readOnlyDictionary != null)
                    {
                        keys = readOnlyDictionary.Keys;
                    }
                    else
                    {
                        keys = ((IDictionary<TKey, TValue>)InnerCollection).Keys;
                    }

                    _keys = keys
                        .WrapToSet(Comparer, () => IsImmutable)
                        .ToReadOnly();
                }

                return (ICollection<TKey>)_keys;
            }
        }

        /// <inheritdoc/>
        public ICollection<TValue> Values
        {
            get
            {
                IEnumerable<TValue> values;
                var readOnlyDictionary = InnerCollection as IReadOnlyDictionary<TKey, TValue>;
                if (readOnlyDictionary != null)
                {
                    values = readOnlyDictionary.Values;
                }
                else
                {
                    values = ((IDictionary<TKey, TValue>)InnerCollection).Values;
                }

                _values = values.WrapToCollection(() => IsImmutable).ToReadOnly();

                return _values;
            }
        }

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => ((IDictionary<TKey, TValue>)this).Keys;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => ((IDictionary<TKey, TValue>)this).Values;

        /// <inheritdoc/>
        public IEqualityComparer<TKey> Comparer
        {
            get
            {
                IEqualityComparer<TKey> resultComparer = null;
                var bclDictionary = InnerCollection as Dictionary<TKey, TValue>;
                if (bclDictionary != null)
                {
                    resultComparer = bclDictionary.Comparer;
                }
                else
                {
                    var dictionaryEx = InnerCollection as IReadOnlyDictionaryEx<TKey, TValue>;
                    if (dictionaryEx != null)
                    {
                        resultComparer = dictionaryEx.Comparer;
                    }
                }

                if (resultComparer != null && _comparer == null)
                {
                    return resultComparer;
                }

                if (resultComparer == null && _comparer != null)
                {
                    return _comparer;
                }

                if (resultComparer == null && _comparer == null)
                {
                    throw new NotSupportedException("Current BCL collection does not support Comparer property.");
                }

                if (resultComparer != null && _comparer != null && !ReferenceEquals(resultComparer, _comparer))
                {
                    throw new InvalidOperationException("Forced comparer does not equals to infered comparer");
                }

                return _comparer;
            }
        }

        IImmutableSetSlim<TKey> IImmutableDictionarySlim<TKey, TValue>.Keys => _keys.ToImmutable(true);

        IImmutableCollection<TValue> IImmutableDictionarySlim<TKey, TValue>.Values => _values.ToImmutable(true);

        /// <inheritdoc/>
        [CanBeNull]
        public TValue this[[NotNull] TKey key]
        {
            get
            {
                var dictionary = InnerCollection as IDictionary<TKey, TValue>;
                if (dictionary != null)
                {
                    return dictionary[key];
                }

                return ((IReadOnlyDictionary<TKey, TValue>)InnerCollection)[key];
            }

            set
            {
                VerifyIsModifiable();
                ((IDictionary<TKey, TValue>)InnerCollection)[key] = value;
            }
        }

        /// <inheritdoc/>
        public void Add([NotNull] TKey key, [CanBeNull] TValue value)
        {
            VerifyIsModifiable();
            ((IDictionary<TKey, TValue>)InnerCollection).Add(key, value);
        }

        /// <inheritdoc/>
        public bool ContainsKey([NotNull] TKey key)
        {
            var dictionary = InnerCollection as IDictionary<TKey, TValue>;
            if (dictionary != null)
            {
                return dictionary.ContainsKey(key);
            }

            return ((IReadOnlyDictionary<TKey, TValue>)InnerCollection).ContainsKey(key);
        }

        /// <inheritdoc/>
        public bool Remove([NotNull] TKey key)
        {
            VerifyIsModifiable();
            return ((IDictionary<TKey, TValue>)InnerCollection).Remove(key);
        }

        /// <inheritdoc/>
        public bool TryGetValue([NotNull] TKey key, [CanBeNull] out TValue value)
        {
            var dictionary = InnerCollection as IDictionary<TKey, TValue>;
            if (dictionary != null)
            {
                return dictionary.TryGetValue(key, out value);
            }

            return ((IReadOnlyDictionary<TKey, TValue>)InnerCollection).TryGetValue(key, out value);
        }
    }
}