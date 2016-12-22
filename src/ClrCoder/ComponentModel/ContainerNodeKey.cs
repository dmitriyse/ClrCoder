// <copyright file="ContainerNodeKey.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
namespace ClrCoder.ComponentModel
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// <c>Container</c> node key.
    /// </summary>
    [PublicAPI]
    public struct ContainerNodeKey : IEquatable<ContainerNodeKey>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerNodeKey"/> struct.
        /// </summary>
        /// <param name="type">Node target type.</param>
        /// <param name="key">Node extended key.</param>
        public ContainerNodeKey(Type type, [CanBeNull] object key)
        {
            if (!(type != null))
            {
                // ReSharper disable once ExpressionIsAlwaysNull
                Vx.Throw(type, key);
            }

            Type = type;
            Key = key;
        }

        /// <summary>
        /// Node target type.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Node extended key.
        /// </summary>
        /// <remarks>
        /// User new <see langword="object"/>() for hidden nodes.
        /// </remarks>
        [CanBeNull]
        public object Key { get; }

        /// <summary>
        /// Equality <c>operator</c>.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns><see langword="true"/>, if operands equals.</returns>
        public static bool operator ==(ContainerNodeKey left, ContainerNodeKey right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Inequality <c>operator</c>.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns><see langword="true"/>, if operands deffer.</returns>
        public static bool operator !=(ContainerNodeKey left, ContainerNodeKey right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc/>
        public bool Equals(ContainerNodeKey other)
        {
            return Type == other.Type && Equals(Key, other.Key);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is ContainerNodeKey && Equals((ContainerNodeKey)obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((Type?.GetHashCode() ?? 0) * 397) ^ (Key?.GetHashCode() ?? 0);
            }
        }
    }
}