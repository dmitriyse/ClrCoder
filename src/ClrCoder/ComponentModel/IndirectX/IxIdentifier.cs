// <copyright file="IxIdentifier.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// Identifies component inside scope.
    /// Scope can contains more that one components with the same type, in <c>this</c> case <see cref="Name"/> will helps to
    /// strongly identify component.
    /// </summary>
    [PublicAPI]
    public struct IxIdentifier : IEquatable<IxIdentifier>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IxIdentifier"/> struct.
        /// </summary>
        /// <param name="type">Type part of identifier.</param>
        /// <param name="name">Additional name.</param>
        public IxIdentifier(Type type, [CanBeNull] string name = null)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            Type = type;
            Name = name;
        }

        /// <summary>
        /// Type part of identifier.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Specifies additional name.
        /// </summary>
        [CanBeNull]
        public string Name { get; }

        /// <inheritdoc/>
        public bool Equals(IxIdentifier other)
        {
            return Type == other.Type && string.Equals(Name, other.Name);
        }

        /// <inheritdoc/>
        public override bool Equals([CanBeNull] object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is IxIdentifier && Equals((IxIdentifier)obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((Type?.GetHashCode() ?? 0) * 397) ^ (Name?.GetHashCode() ?? 0);
            }
        }

        /// <summary>
        /// Equality <c>operator</c>.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns><see langword="true"/> if operands equal, <see langword="false"/> otherwise.</returns>
        public static bool operator ==(IxIdentifier left, IxIdentifier right)
        {
            return left.Type == right.Type && left.Name == right.Name;
        }

        /// <summary>
        /// Inequality <c>operator</c>.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns><see langword="true"/> if operands differs, <see langword="false"/> otherwise.</returns>
        public static bool operator !=(IxIdentifier left, IxIdentifier right)
        {
            return left.Type != right.Type || left.Name != right.Name;
        }
    }
}