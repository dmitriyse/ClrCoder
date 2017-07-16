// <copyright file="IClusterIoMessageBuilder.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Cluster.IO
{
    using BigMath;

    public interface IClusterIoMessageBuilder
    {
        void AddFutureParam(Int128 futureId);

        void AddParam<T>(T data);

        void AddPromiseParam(Int128 promiseId);
    }
}