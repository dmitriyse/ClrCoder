// <copyright file="IFreezable.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System
{
    using JetBrains.Annotations;

    /// <summary>
    /// Alias for <see cref="IFreezable{T}"/>&lt;<see cref="object"/>&gt;, shows that object with any it's aspect can be
    /// frozen.
    /// </summary>
    [PublicAPI]
    public interface IFreezable : IFreezable<object>
    {
    }
}