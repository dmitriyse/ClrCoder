// <copyright file="EnumerableWrappingEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Linq
{
    using Collections.Generic;

    using JetBrains.Annotations;

    /// <summary>
    /// Linq extension methods related to collections wrapping.
    /// </summary>
    [PublicAPI]
    public static class EnumerableWrappingEx
    {
        /// <summary>
        /// Converts collection of items to the hashset.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the enumeration.</typeparam>
        /// <param name="enumerable">Enumeration to convert to hashset.</param>
        /// <returns><see cref="HashSetEx{T}"/> with items from the enumeration.</returns>
        public static HashSetEx<T> ToHashSet<T>(IEnumerable<T> enumerable)
        {
            return new HashSetEx<T>(enumerable);
        }

        public static ISetEx<T> ToReadOnly<T>(this ISet<T> set)
        {
            throw new NotImplementedException();
        }

        public static ICollectionEx<T> ToReadOnly<T>(this ICollection<T> collection)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Wraps enumeration to collection with most effective way, probably cast.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the enumeration.</typeparam>
        /// <param name="enumerable">Enumeration to wrap to collection.</param>
        /// <returns>Wrapped enumeration that provides <see cref="ICollectionEx{T}"/> contract.</returns>
        public static ICollectionEx<T> WrapToCollection<T>(this IEnumerable<T> enumerable, Func<bool> isImmutable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            var wrapOperation = enumerable as IWrapLinqOperation;

            if (wrapOperation == null)
            {
                var transitiveSourceOp = enumerable as ITransitiveSourceLinqOperation;
                if (transitiveSourceOp?.Source is IEnumerable<T>)
                {
                    wrapOperation = transitiveSourceOp.Source as IWrapLinqOperation;
                }
            }
            
            // TODO: Impelment apply isImmutable.
            if (wrapOperation != null
                && (wrapOperation.AssumeImmutable || wrapOperation.AssumeImmutable || wrapOperation.Comparer != null))
            {
                // TODO: Implement optimization logic here.
                return new CollectionExtender<T, IEnumerable<T>>(
                    (IEnumerable<T>)wrapOperation.Source,
                    wrapOperation.AssumeImmutable,
                    wrapOperation.AssumeIsReadOnly,
                    wrapOperation.Comparer);
            }

            var collectionEx = enumerable as ICollectionEx<T>;
            if (collectionEx != null)
            {
                return collectionEx;
            }

            return new CollectionExtender<T, IEnumerable<T>>(enumerable);
        }

        public static ISetEx<T> WrapToSet<T>(
            this IEnumerable<T> enumerable,
            IEqualityComparer<T> comparer = null,
            Func<bool> isImmutable = null)
        {
            throw new NotImplementedException();
        }

    }
}