// <copyright file="EntityKey.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.DomainModel
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// Base entity key implementation.
    /// </summary>
    /// <typeparam name="T">Type of key content.</typeparam>
    /// <typeparam name="TKey">Final key type.</typeparam>
    [Immutable]
    [PublicAPI]
    public abstract class EntityKey<T, TKey> : IEntityKey<TKey>, ISimpleEntityKey<T>
        where TKey : EntityKey<T, TKey>
        where T : IEquatable<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityKey{T,TKey}"/> class.
        /// </summary>
        /// <param name="id">Identifier value.</param>
        protected EntityKey(T id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            Id = id;
        }

        /// <summary>
        /// Identifier value.
        /// </summary>
        public T Id { get; }

        /// <summary>
        /// Compares equality of two keys.
        /// </summary>
        /// <param name="left">Left equality comparison operand.</param>
        /// <param name="right">Right equality comparison operand.</param>
        /// <returns><see langword="true"/>, if operand equals, <see langword="false"/> otherwise.</returns>
        public static bool operator ==(EntityKey<T, TKey> left, EntityKey<T, TKey> right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Compares inequality of two keys.
        /// </summary>
        /// <param name="left">Left inequality comparison operand.</param>
        /// <param name="right">Right inequality comparison operand.</param>
        /// <returns><see langword="true"/>, if operand inequals, <see langword="false"/> otherwise.</returns>
        public static bool operator !=(EntityKey<T, TKey> left, EntityKey<T, TKey> right)
        {
            return !Equals(left, right);
        }

        /// <inheritdoc/>
        public bool Equals([CanBeNull] TKey other)
        {
            if (other == null)
            {
                return false;
            }

            return Id.Equals(other.Id);
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

            return Id.Equals(((EntityKey<T, TKey>)obj).Id);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}