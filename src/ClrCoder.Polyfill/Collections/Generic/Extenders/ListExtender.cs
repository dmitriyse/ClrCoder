// <copyright file="ListExtender.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Collections.Generic
{
    using JetBrains.Annotations;

    using Linq;

    /// <summary>
    /// Extends any enumerable to a list and provides maximum possible features. Throws runtime exceptions for features that
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
    [PublicAPI]
    internal class ListExtender<T, TCollection> : CollectionExtender<T, TCollection>,
                                                  IImmutableListSlim<T>,
                                                  IListEx<T>
        where TCollection : IEnumerable<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListExtender{T, TCollection}"/> class.
        /// TODO: Move to static methods allow to optimize wrappings.
        /// </summary>
        /// <param name="innerCollection">Inner collection to wrap.</param>
        /// <param name="assumeImmutable">Assumes that inner collection is in immutable state.</param>
        /// <param name="assumeReadOnly">Assumes that inner collection is readonly.</param>
        public ListExtender(
            TCollection innerCollection,
            bool assumeImmutable = false,
            bool assumeReadOnly = false)
            : base(innerCollection, assumeImmutable, assumeReadOnly)
        {
        }

        /// <inheritdoc/>
        public T this[int index]
        {
            get
            {
                var readOnlyList = InnerCollection as IReadOnlyList<T>;
                if (readOnlyList != null)
                {
                    return readOnlyList[index];
                }

                var list = InnerCollection as IList<T>;
                if (list != null)
                {
                    return list[index];
                }

                // Fallback to emulation.
                if (index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return InnerCollection.Skip(index).First();
            }

            set
            {
                VerifyIsModifiable();

                ((IList<T>)InnerCollection)[index] = value;
            }
        }

        T IImmutableListSlim<T>.this[int index]
        {
            get
            {
                return this[index];
            }
        }

        /// <inheritdoc/>
        public override bool IsReadOnly => !(InnerCollection is IList<T>) || base.IsReadOnly;

        /// <inheritdoc/>
        public int IndexOf([CanBeNull] T item)
        {
            var list = InnerCollection as IList<T>;
            if (list != null)
            {
                return list.IndexOf(item);
            }

            // Fallback to linq emulation.
            int index = InnerCollection.TakeWhile(x => !x.Equals(item)).Count();
            if (index == InnerCollection.Count())
            {
                index = -1;
            }

            return index;
        }

        /// <inheritdoc/>
        public void Insert(int index, [CanBeNull] T item)
        {
            VerifyIsModifiable();

            ((IList<T>)InnerCollection).Insert(index, item);
        }

        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            VerifyIsModifiable();
            ((IList<T>)InnerCollection).RemoveAt(index);
        }
    }
}