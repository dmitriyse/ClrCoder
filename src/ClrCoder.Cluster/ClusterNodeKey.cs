// <copyright file="ClusterNodeKey.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Cluster
{
    using System;

    using JetBrains.Annotations;

    using Validation;

    /// <summary>
    /// </summary>
    public class ClusterNodeKey : IEquatable<ClusterNodeKey>
    {
        public ClusterNodeKey(string code)
        {
            VxArgs.NonNullOrWhiteSpace(code, nameof(code));
            Code = code;
        }

        /// <summary>
        /// Human memorable short node identifier.
        /// </summary>
        /// <remarks>
        /// Something like dc1-role2-03. Where 03 is a machine number. Example azure-europe-worker-05.
        /// </remarks>
        public string Code { get; }

        public static bool operator ==(ClusterNodeKey left, ClusterNodeKey right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ClusterNodeKey left, ClusterNodeKey right)
        {
            return !Equals(left, right);
        }

        /// <inheritdoc/>
        public bool Equals([CanBeNull] ClusterNodeKey other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(Code, other.Code);
        }

        /// <inheritdoc/>
        public override bool Equals([CanBeNull] object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((ClusterNodeKey)obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }
    }
}