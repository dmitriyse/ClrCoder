// <copyright file="RuntimeLocalRef.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Cluster
{
    /// <summary>
    /// Simple reference to local runtime object.
    /// </summary>
    /// <typeparam name="T">The type of the reference target.</typeparam>
    public class RuntimeLocalRef<T> : IClusterRef<T>
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeLocalRef{T}"/> class.
        /// </summary>
        /// <param name="target">The reference target.</param>
        public RuntimeLocalRef(T target)
        {
            Target = target;
        }

        /// <summary>
        /// The reference target.
        /// </summary>
        public T Target { get; }
    }
}