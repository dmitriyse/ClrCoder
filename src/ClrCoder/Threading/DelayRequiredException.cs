// <copyright file="DelayRequiredException.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Error raised when some operation can be retried after some delay.
    /// </summary>
    public class DelayRequiredException : AlternativeResult<Task>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DelayRequiredException"/> class.
        /// </summary>
        /// <param name="result">Task that will completes when retry can be performed.</param>
        public DelayRequiredException(Task result)
            : base(result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }
        }
    }
}