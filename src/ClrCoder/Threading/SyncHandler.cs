// <copyright file="SyncHandler.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// Handles work items synchronously.
    /// </summary>
    public class SyncHandler : IAsyncHandler
    {
        /// <inheritdoc/>
        public void RunAsync(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            try
            {
                action();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <inheritdoc/>
        public void RunAsync<T>(Action<T> action, [CanBeNull] T arg)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            try
            {
                action(arg);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <inheritdoc/>
        public void RunAsync<T1, T2>(Action<T1, T2> action, [CanBeNull] T1 arg1, [CanBeNull] T2 arg2)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            try
            {
                action(arg1, arg2);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <inheritdoc/>
        public void RunAsync<T1, T2, T3>(
            Action<T1, T2, T3> action,
            [CanBeNull] T1 arg1,
            [CanBeNull] T2 arg2,
            [CanBeNull] T3 arg3)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            try
            {
                action(arg1, arg2, arg3);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <inheritdoc/>
        public void RunAsync<T1, T2, T3, T4>(
            Action<T1, T2, T3, T4> action,
            [CanBeNull] T1 arg1,
            [CanBeNull] T2 arg2,
            [CanBeNull] T3 arg3,
            [CanBeNull] T4 arg4)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            try
            {
                action(arg1, arg2, arg3, arg4);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <inheritdoc/>
        public void RunAsync<T1, T2, T3, T4, T5>(
            Action<T1, T2, T3, T4, T5> action,
            [CanBeNull] T1 arg1,
            [CanBeNull] T2 arg2,
            [CanBeNull] T3 arg3,
            [CanBeNull] T4 arg4,
            [CanBeNull] T5 arg5)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            try
            {
                action(arg1, arg2, arg3, arg4, arg5);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <inheritdoc/>
        public void RunAsync<T1, T2, T3, T4, T5, T6>(
            Action<T1, T2, T3, T4, T5, T6> action,
            [CanBeNull] T1 arg1,
            [CanBeNull] T2 arg2,
            [CanBeNull] T3 arg3,
            [CanBeNull] T4 arg4,
            [CanBeNull] T5 arg5,
            [CanBeNull] T6 arg6)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            try
            {
                action(arg1, arg2, arg3, arg4, arg5, arg6);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        /// <inheritdoc/>
        public void RunAsync<T1, T2, T3, T4, T5, T6, T7>(
            Action<T1, T2, T3, T4, T5, T6, T7> action,
            [CanBeNull] T1 arg1,
            [CanBeNull] T2 arg2,
            [CanBeNull] T3 arg3,
            [CanBeNull] T4 arg4,
            [CanBeNull] T5 arg5,
            [CanBeNull] T6 arg6,
            [CanBeNull] T7 arg7)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            try
            {
                action(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void HandleException(Exception ex)
        {
            AppDomainEx.RaiseNonTerminatingUnhandledException(ex);
        }
    }
}