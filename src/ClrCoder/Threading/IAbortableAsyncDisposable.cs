// <copyright file="IAbortableAsyncDisposable.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    using System;

    /// <summary>
    /// Allows to detect and exception that was raised inside AsyncUsing block.
    /// </summary>
    public interface IAbortableAsyncDisposable : IAsyncDisposable
    {
        /// <summary>
        /// Handles <c>exception</c> raised from an AsyncUsing block.
        /// </summary>
        /// <param name="exception">Raised <c>exception</c>.</param>
        void HandleException(Exception exception);
    }
}