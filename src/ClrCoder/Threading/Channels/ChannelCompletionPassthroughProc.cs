// <copyright file="ChannelCompletionPassthroughProc.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading.Channels
{
    using System;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// Completion passthrough handler delegate.
    /// </summary>
    /// <param name="error">The error to passthrough.</param>
    /// <param name="isCompletionFromChannel">
    /// <see langword="true"/>, if error was received from channel,
    /// <see langword="false"/> - error was raised from processing code.
    /// </param>
    /// <returns>The async execution TPL task.</returns>
    public delegate ValueTask ChannelCompletionPassthroughProc(
        [CanBeNull] Exception error = null,
        bool isCompletionFromChannel = true);
}