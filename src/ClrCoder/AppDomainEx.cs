// <copyright file="AppDomainEx.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// Extended functionality related to the "AppDomain".
    /// </summary>
    [PublicAPI]
    public static class AppDomainEx
    {
        static AppDomainEx()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
        }

        /// <summary>
        /// Fires on terminating and non terminating unhandled exceptions.
        /// </summary>
        public static event EventHandler<UnhandledExceptionEventArgsEx> UnhandledException;

        /// <summary>
        /// Raises unhandled exception that will not terminate application.
        /// </summary>
        /// <param name="ex">The unhandled exception to rise.</param>
        public static void RaiseNonTerminatingUnhandledException(Exception ex)
        {
            UnhandledException?.Invoke(AppDomain.CurrentDomain, new UnhandledExceptionEventArgsEx(ex, false));
        }

        private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            UnhandledException?.Invoke(sender, new UnhandledExceptionEventArgsEx(e.ExceptionObject, e.IsTerminating));
        }
    }
}