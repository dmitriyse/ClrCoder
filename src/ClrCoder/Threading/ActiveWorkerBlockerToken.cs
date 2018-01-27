// <copyright file="ActiveWorkerBlockerToken.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// The <see cref="ActiveWorker"/> work blocker token. This structure must be
    /// </summary>
    public struct ActiveWorkerBlockerToken : IDisposable
    {
        private readonly ActiveWorker.WorkBlocker _workBlocker;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveWorkerBlockerToken"/> struct.
        /// </summary>
        /// <param name="workBlocker">The work blocker.</param>
        internal ActiveWorkerBlockerToken(ActiveWorker.WorkBlocker workBlocker)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            Debug.Assert(workBlocker != null, "workBlocker != null");

            _workBlocker = workBlocker;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_workBlocker == null)
            {
                throw new InvalidOperationException("You cannot use uninitialized ActiveWorkerBlockerToken variable.");
            }

            _workBlocker.Dispose();
        }
    }
}