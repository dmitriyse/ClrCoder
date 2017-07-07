// <copyright file="ClusterNodeKey.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Cluster
{
    /// <summary>
    /// </summary>
    public class ClusterNodeKey
    {
        /// <summary>
        /// Human memorable short node identifier.
        /// </summary>
        /// <remarks>
        /// Something like dc1-role2-03. Where 03 is a machine number. Example azure-europe-worker-05.
        /// </remarks>
        public string Code { get; set; }
    }
}