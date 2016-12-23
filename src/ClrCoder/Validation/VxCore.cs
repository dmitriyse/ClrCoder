// <copyright file="VxCore.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Validation
{
    /// <summary>
    /// Validation class.
    /// </summary>
    public static class VxCore
    {
        /// <summary>
        /// Generic validation <c>throw</c> method.
        /// </summary>
        /// <param name="callerFilePath">Calling member file path.</param>
        /// <param name="callerLineNumber">Calling member line number.</param>
        /// <param name="args">Method arguments that was passed to failed method.</param>
        public static void Throw(string callerFilePath, int callerLineNumber, params object[] args)
        {
            throw new VxValidationException(callerFilePath, callerLineNumber, args);
        }
    }
}