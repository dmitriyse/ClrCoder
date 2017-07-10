// <copyright file="ValueVoid.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;

    using JetBrains.Annotations;

    /// <summary>
    /// Special type that can be used in places where usage of System.Void currently is impossible.
    /// </summary>
    /// <remarks>
    /// See discussion https://github.com/dotnet/csharplang/issues/696.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    [PublicAPI]
    public struct ValueVoid : IEquatable<ValueVoid>, IStructuralEquatable
    {
        /// <inheritdoc/>
        public bool Equals(ValueVoid other)
        {
            return true;
        }

        /// <inheritdoc/>
        public override bool Equals([CanBeNull] object obj)
        {
            return obj is ValueVoid;
        }

        bool IStructuralEquatable.Equals([CanBeNull] object other, [CanBeNull] IEqualityComparer comparer)
        {
            return other is ValueTuple;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return 0;
        }

        int IStructuralEquatable.GetHashCode([CanBeNull] IEqualityComparer comparer)
        {
            return 0;
        }
    }
}