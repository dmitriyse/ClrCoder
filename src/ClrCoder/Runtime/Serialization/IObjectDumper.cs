// <copyright file="IObjectDumper.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Runtime.Serialization
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// Dumper contract, used in hierarchy dump algorithm.
    /// </summary>
    public interface IObjectDumper
    {
        /// <summary>
        /// Performs part of dumping. This method is highly reenterant.
        /// </summary>
        /// <param name="type">Type of filter to apply.</param>
        /// <param name="dump">Dump to apply filter on, if <see langword="null"/>, <c>dump</c> should be created.</param>
        /// <param name="target">Target object to dump.</param>
        /// <returns>Filtered dump.</returns>
        object Filter(Type type, [CanBeNull] object dump, object target);

        /////// <summary>
        /////// Tries to get already known dumped object.
        /////// </summary>
        /////// <param name="target"></param>
        /////// <param name="dumped"></param>
        /////// <returns></returns>
        ////bool TryGetDumped(object target, out object dumped);
    }

    /// <summary>
    /// Dump filter. Receives <c>dump</c> <c>object</c> and <c>dump</c> <c>target</c> <c>object</c>, do some dumping and
    /// delegates related work to provided <c>dumper</c>.
    /// </summary>
    /// <param name="queriedType">Queried type, <paramref name="target"/> always derived or equal to this type.</param>
    /// <param name="dump">Dump to fill. If <see langword="null"/>, dump should be created.</param>
    /// <param name="target">Target object to dump.</param>
    /// <param name="dumper">Dump operation object.</param>
    /// <returns>Filtered dump.</returns>
    public delegate object DumpObjectFilterDelegate(
        Type queriedType,
        [CanBeNull] object dump,
        object target,
        IObjectDumper dumper);
}