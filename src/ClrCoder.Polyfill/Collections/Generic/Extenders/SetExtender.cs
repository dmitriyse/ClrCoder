// <copyright file="SetExtender.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Collections.Generic
{
    using JetBrains.Annotations;

    using Linq;

    /// <summary>
    /// Extends any enumerable to a set and provides maximum possible features. Throws runtime exceptions for features that
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
    internal class SetExtender<T, TCollection> : CollectionExtender<T, TCollection>
        where TCollection : IEnumerable<T>
    {
        private readonly IEqualityComparer<T> _comparer;

        private HashSetEx<T> _clone;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetExtender{T, TCollection}"/> class.
        /// TODO: Move to static methods allow to optimize wrappings.
        /// </summary>
        /// <param name="innerCollection">Inner collection to wrap.</param>
        /// <param name="assumeImmutable">Assumes that inner collection is in immutable state.</param>
        /// <param name="assumeReadOnly">Assumes that inner collection is readonly.</param>
        /// <param name="comparer">Assumes that set uses this comparer.</param>
        public SetExtender(
            TCollection innerCollection,
            bool assumeImmutable = false,
            bool assumeReadOnly = false,
            IEqualityComparer<T> comparer = null)
            : base(innerCollection, assumeImmutable, assumeReadOnly, comparer)
        {
            _comparer = comparer;

#if DEBUG
            if (!(innerCollection is ISet<T>))
            {
                if (innerCollection.Distinct(Comparer).Count() != Count)
                {
                    throw new ArgumentException("Cannot extend not unique collection to set.");
                }
            }

#endif
        }

        /// <inheritdoc/>
        public override bool IsReadOnly => !(InnerCollection is ISet<T>) || base.IsReadOnly;

        /// <inheritdoc/>
        public IEqualityComparer<T> Comparer
        {
            get
            {
                IEqualityComparer<T> resultComparer = null;
                var bclSet = InnerCollection as HashSet<T>;
                if (bclSet != null)
                {
                    resultComparer = bclSet.Comparer;
                }
                else
                {
                    var setEx = InnerCollection as ISetEx<T>;
                    if (setEx != null)
                    {
                        resultComparer = setEx.Comparer;
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

        /// <inheritdoc/>
        public new bool Add([NotNull] T item)
        {
            VerifyIsModifiable();
            return ((ISet<T>)InnerCollection).Add(item);
        }

        /// <inheritdoc/>
        public void ExceptWith([NotNull] IEnumerable<T> other)
        {
            VerifyIsModifiable();
            ((ISet<T>)InnerCollection).ExceptWith(other);
        }

        /// <inheritdoc/>
        public void IntersectWith([NotNull] IEnumerable<T> other)
        {
            VerifyIsModifiable();
            ((ISet<T>)InnerCollection).IntersectWith(other);
        }

        /// <inheritdoc/>
        public bool IsProperSubsetOf([NotNull] IEnumerable<T> other)
        {
            return GetSetForRead().IsProperSubsetOf(other);
        }

        /// <inheritdoc/>
        public bool IsProperSupersetOf([NotNull] IEnumerable<T> other)
        {
            return GetSetForRead().IsProperSupersetOf(other);
        }

        /// <inheritdoc/>
        public bool IsSubsetOf([NotNull] IEnumerable<T> other)
        {
            return GetSetForRead().IsSubsetOf(other);
        }

        /// <inheritdoc/>
        public bool IsSupersetOf([NotNull] IEnumerable<T> other)
        {
            return GetSetForRead().IsSupersetOf(other);
        }

        /// <inheritdoc/>
        public bool Overlaps([NotNull] IEnumerable<T> other)
        {
            return GetSetForRead().Overlaps(other);
        }

        /// <inheritdoc/>
        public bool SetEquals([NotNull] IEnumerable<T> other)
        {
            return GetSetForRead().SetEquals(other);
        }

        /// <inheritdoc/>
        public void SymmetricExceptWith([NotNull] IEnumerable<T> other)
        {
            VerifyIsModifiable();
            ((ISet<T>)InnerCollection).SymmetricExceptWith(other);
        }

        /// <inheritdoc/>
        public void UnionWith([NotNull] IEnumerable<T> other)
        {
            VerifyIsModifiable();
            ((ISet<T>)InnerCollection).UnionWith(other);
        }

        private ISet<T> GetSetForRead()
        {
            var innerSet = InnerCollection as ISet<T>;
            if (innerSet != null)
            {
                return innerSet;
            }

            if (IsImmutable)
            {
                return _clone ?? (_clone = new HashSetEx<T>(InnerCollection, Comparer));
            }

            return new HashSetEx<T>(InnerCollection, Comparer);
        }
    }
}