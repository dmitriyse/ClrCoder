// <copyright file="CollectionExtender.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Collections.Generic
{
    using JetBrains.Annotations;

    using Linq;

    /// <summary>
    /// Extends any enumerable to a collection and provides maximum possible features. Throws runtime exceptions for features
    /// that
    /// are impossible to provide.
    /// </summary>
    /// <remarks>
    /// Also class helps to emulate
    /// proposals https://github.com/dotnet/corefx/issues/16626, https://github.com/dotnet/corefx/issues/16661 on current BCL.
    /// TCollection parameter allows compiler to generate more efficient code.
    /// </remarks>
    /// <typeparam name="T">The type of the item in the collection.</typeparam>
    /// <typeparam name="TCollection">The type of the inner collection.</typeparam>
    /// <filterpriority>1</filterpriority>
    internal class CollectionExtender<T, TCollection> : ICollectionEx<T>, IWrapLinqOperation
        where TCollection : IEnumerable<T>
    {
        private readonly bool _isConcludedImmutable;

        private readonly bool _isConcludedReadOnly;

        private readonly object _comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionExtender{T, TCollection}"/> class.
        /// TODO: Move to static methods allow to optimize wrappings.
        /// </summary>
        /// <param name="innerCollection">Inner collection to wrap.</param>
        /// <param name="assumeImmutable">Assumes that inner collection is in immutable state.</param>
        /// <param name="assumeReadOnly">Assumes that inner collection is readonly.</param>
        /// <param name="comparer">Assumes that set uses this comparer.</param>
        public CollectionExtender(
            TCollection innerCollection,
            bool assumeImmutable = false,
            bool assumeReadOnly = false,
            [CanBeNull] object comparer = null)
        {
            _comparer = comparer;

            if (innerCollection == null)
            {
                throw new ArgumentNullException(nameof(innerCollection));
            }

            AssumeImmutable = assumeImmutable;
            AssumeIsReadOnly = AssumeIsReadOnly;
            _isConcludedImmutable = AssumeImmutable;

            InnerCollection = innerCollection;

            _isConcludedReadOnly = assumeReadOnly || !(innerCollection is ICollection<T>);
        }

        /// <inheritdoc/>
        public bool AssumeImmutable { get; }

        /// <inheritdoc/>
        public bool AssumeIsReadOnly { get; }

        /// <inheritdoc/>
        public virtual bool IsImmutable
        {
            get
            {
                if (IsImmutableOverride != null)
                {
                    return IsImmutableOverride();
                }

                // ReSharper disable once SuspiciousTypeConversion.Global
                var freezable = InnerCollection as IFreezable<IEnumerable<T>>;
                if (freezable != null)
                {
                    if (freezable.IsFrozen)
                    {
                        return true;
                    }
                }

                if (_isConcludedImmutable)
                {
                    return true;
                }

                // ReSharper disable once SuspiciousTypeConversion.Global
                return (InnerCollection as IImmutable<T>)?.IsImmutable ?? false;
            }
        }

        /// <inheritdoc/>
        public int Count
        {
            get
            {
                var readOnlyCollection = (IReadOnlyCollection<T>)InnerCollection;
                if (readOnlyCollection != null)
                {
                    return readOnlyCollection.Count;
                }

                var collection = (ICollection<T>)InnerCollection;
                if (collection != null)
                {
                    return collection.Count;
                }

                // TODO: Add support for other well-known collections like stack, queue.

                // Fallback to linq count.
                return InnerCollection.Count();
            }
        }

        /// <inheritdoc/>
        public virtual bool IsReadOnly
        {
            get
            {
                if (IsImmutable || _isConcludedReadOnly)
                {
                    return true;
                }

                return ((ICollection<T>)InnerCollection).IsReadOnly;
            }
        }

        /// <inheritdoc/>
        object IWrapLinqOperation.Comparer => _comparer;

        /// <inheritdoc/>
        public IEnumerable Source => InnerCollection;

        /// <inheritdoc/>
        public Func<bool> IsImmutableOverride { get; set; }

        /// <summary>
        /// The inner collection.
        /// </summary>
        public TCollection InnerCollection { get; }

        /// <inheritdoc/>
        public void Add([CanBeNull] T item)
        {
            VerifyIsModifiable();
            ((ICollection<T>)InnerCollection).Add(item);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            VerifyIsModifiable();
            ((ICollection<T>)InnerCollection).Clear();
        }

        /// <inheritdoc/>
        public bool Contains([CanBeNull] T item)
        {
            return ((ICollection<T>)InnerCollection).Contains(item);
        }

        /// <inheritdoc/>
        public void CopyTo([NotNull] T[] array, int arrayIndex)
        {
            ((ICollection<T>)InnerCollection).CopyTo(array, arrayIndex);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return InnerCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)InnerCollection).GetEnumerator();
        }

        /// <inheritdoc/>
        public bool Remove([CanBeNull] T item)
        {
            VerifyIsModifiable();
            return ((ICollection<T>)InnerCollection).Remove(item);
        }

        /// <summary>
        /// Verifies that collection is not in the immutable state.
        /// </summary>
        protected void VerifyIsModifiable()
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException(
                    "Cannot modify the collection that it is in immutable or readonly state.");
            }
        }
    }
}