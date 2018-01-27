// <copyright file="IActiveWorkItem.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
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
        /// <param name="debugInfo">The work blocker debug information.</param>
        /// <param name="filePath">The source code file path of the work blocker section enter.</param>
        /// <param name="memberName">The class member name of the work blocker section enter.</param>
        /// <param name="lineNumber">The line number of the work blocker section enter.</param>
        /// <remarks>
        /// Once work item exits from a first blocker section, new work item is created.
        /// </remarks>
        /// <returns>The work blocker exit token.</returns>
        ActiveWorkerBlockerToken EnterWorkBlocker(
            string debugInfo = "",
            [CallerFilePath] string filePath = null,
            [CallerMemberName] string memberName = null,
            [CallerLineNumber] int lineNumber = 0);
    }
}