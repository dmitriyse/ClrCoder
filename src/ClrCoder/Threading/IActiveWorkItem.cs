// <copyright file="IActiveWorkItem.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    using System;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    /// <summary>
    /// Active work item.
    /// </summary>
    [PublicAPI]
    public interface IActiveWorkItem
    {
        /// <summary>
        /// Enters into work blocker section. Section where new work items cannot be created.
        /// </summary>
        /// <param name="debugInfo">Work blocker debug information.</param>
        /// <param name="filePath">Enter place source code file path.</param>
        /// <param name="memberName">Enter place member name.</param>
        /// <param name="lineNumber">Enter place line number.</param>
        /// <remarks>
        /// Once work item exits from a first blocker section, new work item is created.
        /// </remarks>
        /// <returns>Work blocker exit token.</returns>
        IDisposable EnterWorkBlocker(
            string debugInfo = "",
            [CallerFilePath] string filePath = null,
            [CallerMemberName] string memberName = null,
            [CallerLineNumber] int lineNumber = 0);
    }
}