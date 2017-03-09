// <copyright file="IReadOnlySet.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System.Collections.Generic
{
    /// <summary>
    /// Readable set abstracton. Allows fast contains method, also shows that collection items are unique by some criteria.
    /// </summary>
    /// <remarks>
    /// Proposal for this abstraction is discussed here https://github.com/dotnet/corefx/issues/1973.
    /// </remarks>
    /// <typeparam name="T">The type of elements in the set.</typeparam>
    public interface IReadOnlySet<out T> : IReadOnlyCollection<T>
    {
        /// <summary>
        /// Determines whether a <see cref="T:System.Collections.Generic.HashSet`1"/> object contains the specified
        /// element.
        /// </summary>
        /// <typeparam name="TItem">The type of the provided item. This trick allows to save contravariance and save from boxing.</typeparam>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.HashSet`1"/> object contains the specified element;
        /// otherwise, false.
        /// </returns>
        /// <param name="item">The element to locate in the <see cref="T:System.Collections.Generic.HashSet`1"/> object.</param>
        bool Contains<TItem>(TItem item);
    }
}