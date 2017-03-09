// <copyright file="ImmutabilityExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System
{
    using JetBrains.Annotations;

    /// <summary>
    /// Various immutability helper extension methods.
    /// </summary>
    [PublicAPI]
    public static class ImmutabilityExtensions
    {
        /// <summary>
        /// Verifies, that the provided object is immutable.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="obj">The object to verify.</param>
        /// <returns><see langword="true"/>, object considered as currently immutable, <see langword="false"/> otherwise.</returns>
        public static bool IsImmutable<T>([CanBeNull] this T obj)
        {
            if (obj == null)
            {
                return true;
            }

            if (obj is IImmutable<T>)
            {
                return true;
            }

            var immutableState = obj as IImmutableState<T>;
            return immutableState?.IsImmutable ?? false;
        }

        /// <summary>
        /// Verifies, that the provided object is shallow immutable.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="obj">The object to verify.</param>
        /// <returns><see langword="true"/>, object considered as currently shallow immutable, <see langword="false"/> otherwise.</returns>
        public static bool IsShallowImmutable<T>([CanBeNull] this T obj)
        {
            if (obj == null)
            {
                return true;
            }

            if (obj is IShallowImmutable<T>)
            {
                return true;
            }

            var immutableState = obj as IImmutableState<T>;
            return immutableState?.IsImmutable ?? false;
        }

        /// <summary>
        /// Tries to freeze object when it's inherit appropriate interface.
        /// </summary>
        /// <typeparam name="T">The type of part/aspect is to be frozen.</typeparam>
        /// <param name="obj">The object to freeze.</param>
        /// <returns><see langword="true"/>, if object becomes frozen.</returns>
        public static bool TryFreeze<T>(this T obj)
        {
            var freezable = obj as IFreezable<T>;
            if (freezable != null)
            {
                freezable.Freeze();
                return true;
            }

            return IsImmutable(obj);
        }

        /// <summary>
        /// Tries to shallow freeze object when it's inherit appropriate interface.
        /// </summary>
        /// <typeparam name="T">The type of part/aspect is to be shallow frozen.</typeparam>
        /// <param name="obj">The object to shallowfreeze.</param>
        /// <returns><see langword="true"/>, if object becomes shallow frozen.</returns>
        public static void TryShallowFreeze<T>(this T obj)
        {
            var freezable = obj as IFreezable<T>;
            freezable?.ShallowFreeze();
        }
    }
}