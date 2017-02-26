// <copyright file="Critical.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder
{
    using System;
#if DEBUG
    using System.Diagnostics;
#endif
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    /// <summary>
    /// Utilities for handling critical application problems.
    /// </summary>
    [PublicAPI]
    public static class Critical
    {
        private static CriticalAssertionFaultHandlerDelegate _assertionFaultHandler = DefaultAssertionHandler;

        /// <summary>
        /// Handles critical assertion faults.
        /// </summary>
        public static CriticalAssertionFaultHandlerDelegate AssertionFaultHandler
        {
            get
            {
                return _assertionFaultHandler;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _assertionFaultHandler = value;
            }
        }

        /// <summary>
        /// Asserts <paramref name="condition"/> is true, and produce critical error otherwise.
        /// </summary>
        /// <param name="condition">Condition to verify.</param>
        /// <param name="message">Assertion fault error description.</param>
        /// <param name="detailMessage">Assertion fault details.</param>
        /// <param name="fileName">Sources file name where an assertion faulted./</param>
        /// <param name="memberName">Member name where an assertion faulted.</param>
        /// <param name="lineNumber">Source file line number where an assertion faulted.</param>
        [AssertionMethod]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Assert(
            [AssertionCondition(AssertionConditionType.IS_TRUE)]
            bool condition,
            string message,
            string detailMessage = null,
            [CallerFilePath] string fileName = null,
            [CallerMemberName] string memberName = null,
            [CallerLineNumber] int lineNumber = 0)
        {
            if (!condition)
            {
                // ReSharper disable AssignNullToNotNullAttribute
                HandleAssertionFault(message, detailMessage, fileName, memberName, lineNumber);

                // ReSharper restore AssignNullToNotNullAttribute
            }
        }

        private static void DefaultAssertionHandler(string message, string detailMessage, CallerInfo callerInfo)
        {
#if DEBUG
            Debug.Assert(false, message, detailMessage);
#else
            throw new CriticalAssertionFaultException(message, detailMessage, callerInfo);
#endif
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void HandleAssertionFault(
            string message,
            [CanBeNull] string detailMessage,
            string fileName,
            string member,
            int lineNumber)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (member == null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            AssertionFaultHandler(message, detailMessage, new CallerInfo(fileName, member, lineNumber));
        }
    }

    /// <summary>
    /// <c>Critical</c> assertion fault <see langword="delegate"/>.
    /// </summary>
    /// <param name="message">Assertion message.</param>
    /// <param name="detailMessage">Assertion details.</param>
    /// <param name="callerInfo">Caller info.</param>
    public delegate void CriticalAssertionFaultHandlerDelegate(
        string message,
        [CanBeNull] string detailMessage,
        CallerInfo callerInfo);
}