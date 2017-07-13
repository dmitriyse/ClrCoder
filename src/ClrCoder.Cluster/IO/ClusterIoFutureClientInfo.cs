// <copyright file="ClusterIoFutureClientInfo.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Cluster.IO
{
    public class ClusterIoFutureClientInfo
    {
        public ClusterIoFutureRef Future { get; }

        public ClusterNodeKey ClientNode { get; }
    }
}