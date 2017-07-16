// <copyright file="IClusterIoMessageParam.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Cluster.IO
{
    using BigMath;

    using JetBrains.Annotations;

    /// <summary>
    /// The cluster io message parameter, that can be one of three types: promise, future, data.
    /// </summary>
    public interface IClusterIoMessageParam
    {
        [CanBeNull]
        IClusterIoPromise Promise { get; }

        /// <summary>
        /// The identifier for the local future to create - it will receive a response.
        /// </summary>
        [CanBeNull]
        Int128 FutureId { get; }

        T GetData<T>();
    }
}