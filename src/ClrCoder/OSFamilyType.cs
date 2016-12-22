// <copyright file="OSFamilyType.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
namespace ClrCoder
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// Operation system family type.
    /// </summary>
    [PublicAPI]
    [Flags]
    public enum OSFamilyTypes
    {
        /// <summary>
        /// Portable environment or unknown yet platform.
        /// </summary>
        Portable = 0,

        /// <summary>
        /// Windows OS family.
        /// </summary>
        Windows = 0x1,

        /// <summary>
        /// Linux OS family.
        /// </summary>
        Linux = 0x02 | Posix,

        /// <summary>
        /// Posix OS - any Linux,Unix,MacOS, etc.
        /// </summary>
        Posix = 0x1000,
    }
}