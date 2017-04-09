// <copyright file="IKeyedCollection.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Collections
{
    using System.Diagnostics.CodeAnalysis;

    using ObjectModel;

    /// <summary>
    /// The collection of keyed items.
    /// </summary>
    /// <typeparam name="TKey">The type of the items key.</typeparam>
    /// <typeparam name="TValue">The type of the collection item.</typeparam>
    [SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity", Justification = "Reviewed, ok.")]
    public interface IKeyedCollection<TKey, TValue> : IReadOnlyKeyedCollection<TKey, TValue>
        where TValue : IKeyed<TKey>
    {
    }
}