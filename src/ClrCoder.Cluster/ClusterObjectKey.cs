// <copyright file="ClusterObjectKey.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Cluster
{
    using System;

    using BigMath;

    using Validation;

    public class ClusterObjectKey : IEquatable<ClusterObjectKey>
    {
        public ClusterObjectKey(ClusterNodeKey hostNode, Int128 id)
        {
            VxArgs.NotNull(hostNode, nameof(hostNode));

            HostNode = hostNode;

            Id = id;
        }

        public ClusterNodeKey HostNode { get; }

        public Int128 Id { get; }

        public static bool operator ==(ClusterObjectKey left, ClusterObjectKey right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ClusterObjectKey left, ClusterObjectKey right)
        {
            return !Equals(left, right);
        }

        public bool Equals(ClusterObjectKey other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Id.Equals(other.Id) && HostNode.Equals(other.HostNode);
        }

        public override bool Equals(object obj)
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

            return Equals((ClusterObjectKey)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Id.GetHashCode() * 397) ^ HostNode.GetHashCode();
            }
        }
    }
}