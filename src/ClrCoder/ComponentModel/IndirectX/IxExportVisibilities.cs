// <copyright file="IxVisibilities.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;

    /// <summary>
    /// Registration visibility.
    /// </summary>
    [Flags]
    public enum IxExportVisibilities
    {
        /// <summary>
        /// Registration visible to this scope.
        /// </summary>
        ThisScope = 0,

        /// <summary>
        /// Registration visible to this scope and parent scope.
        /// </summary>
        ParentScope = 1,

        /// <summary>
        /// Registration visible to this scope and all descendants scope.
        /// </summary>
        DescendantScopes = 2
    }
}