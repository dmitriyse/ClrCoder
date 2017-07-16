// <copyright file="IClusterIoPromise.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Cluster.IO
{
    /// <summary>
    /// Client interface to the remote promise wrapper.
    /// </summary>
    public interface IClusterIoPromise
    {
        void SetException();

        void SetResult();
    }
}