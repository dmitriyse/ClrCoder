// <copyright file="IClusterIoObject.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Cluster.IO
{
    using System.Threading.Tasks;

    using ObjectModel;

    /// <summary>
    /// Object should implement this interface to enable cluster remoting.
    /// </summary>
    public interface IClusterIoObject : IKeyed<ClusterObjectKey>
    {
        Task ReceiveCall(string method, IClusterIoMessage message, IClusterIoMessageBuilder responseBuilder);
    }
}