// <copyright file="UnhandledExceptionEventArgsEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System
{
    using JetBrains.Annotations;

    /// <summary>
    /// Provides data for the event that is raised when there is an exception that is not handled in any application domain.
    /// </summary>
    [PublicAPI]
    public class UnhandledExceptionEventArgsEx : EventArgs
    {
        [CanBeNull]
        private object _exception;

        private bool _isTerminating;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnhandledExceptionEventArgsEx"/> class with the exception object and a
        /// common language runtime termination flag.
        /// </summary>
        /// <param name="exception">The exception that is not handled.</param>
        /// <param name="isTerminating">true if the runtime is terminating; otherwise, false.</param>
        public UnhandledExceptionEventArgsEx([CanBeNull] object exception, bool isTerminating)
        {
            _exception = exception;
            _isTerminating = isTerminating;
        }

        /// <summary>
        /// Gets the unhandled exception object.
        /// </summary>
        public object ExceptionObject => _exception;

        /// <summary>
        /// Indicates whether the common language runtime is terminating.
        /// </summary>
        public bool IsTerminating => _isTerminating;
    }
}