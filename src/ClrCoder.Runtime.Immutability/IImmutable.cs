// <copyright file="IImmutable.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System
{
    /// <summary>
    /// Alias for <see cref="IImmutable{T}"/>&lt;<see cref="object"/>&gt;, shows that whole object is fully immutable.
    /// </summary>
    public interface IImmutable : IImmutable<object>, IShallowImmutable
    {
    }
}