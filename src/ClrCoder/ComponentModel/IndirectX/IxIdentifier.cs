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
        [CanBeNull]
        private readonly Type _type;

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

            _type = type;
            Name = name;
        }

        private void EnsureNotDefault()
        {
            if (_type == null)
            {
                throw new InvalidOperationException("Cannot use default IxIdentifier.");
            }
        }

        /// <summary>
        /// Type part of identifier.
        /// </summary>
        public Type Type
        {
            get
            {
                EnsureNotDefault();
                return _type;
            }
        }

        /// <summary>
        /// Specifies additional name.
        /// </summary>
        [CanBeNull]
        public string Name { get; }

        /// <inheritdoc/>
        public bool Equals(IxIdentifier other)
        {
            return _type == other._type && string.Equals(Name, other.Name);
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
                return ((_type?.GetHashCode() ?? 0) * 397) ^ (Name?.GetHashCode() ?? 0);
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
            return left._type == right._type && left.Name == right.Name;
        }

        /// <summary>
        /// Inequality <c>operator</c>.
        /// </summary>
        /// <param name="left">Left operand.</param>
        /// <param name="right">Right operand.</param>
        /// <returns><see langword="true"/> if operands differs, <see langword="false"/> otherwise.</returns>
        public static bool operator !=(IxIdentifier left, IxIdentifier right)
        {
            return left._type != right._type || left.Name != right.Name;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            string result = Type.Name;
            if (!string.IsNullOrWhiteSpace(Name))
            {
                result += "|" + Name;
            }

            return result;
        }
    }
}