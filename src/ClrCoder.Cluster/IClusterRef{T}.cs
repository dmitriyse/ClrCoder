// <copyright file="IClusterRef{T}.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Cluster
{
    /// <summary>
    /// Cluster reference.
    /// </summary>
    /// <typeparam name="T">The type of the reference target.</typeparam>
    public interface IClusterRef<T> : IClusterRef
        where T : class
    {
    }
}