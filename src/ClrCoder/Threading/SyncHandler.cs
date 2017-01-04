// <copyright file="SyncHandler.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    using System;
    using System.Threading;

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
                ThreadPool.QueueUserWorkItem(
                    state => { throw ex; });
            }
        }

        /// <inheritdoc/>
        public void RunAsync<T>(Action<T> action, T arg)
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
                ThreadPool.QueueUserWorkItem(
                    state => { throw ex; });
            }
        }

        /// <inheritdoc/>
        public void RunAsync<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2)
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
                ThreadPool.QueueUserWorkItem(
                    state => { throw ex; });
            }
        }

        /// <inheritdoc/>
        public void RunAsync<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
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
                ThreadPool.QueueUserWorkItem(
                    state => { throw ex; });
            }
        }

        /// <inheritdoc/>
        public void RunAsync<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
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
                ThreadPool.QueueUserWorkItem(
                    state => { throw ex; });
            }
        }

        /// <inheritdoc/>
        public void RunAsync<T1, T2, T3, T4, T5>(
            Action<T1, T2, T3, T4, T5> action,
            T1 arg1,
            T2 arg2,
            T3 arg3,
            T4 arg4,
            T5 arg5)
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
                ThreadPool.QueueUserWorkItem(
                    state => { throw ex; });
            }
        }
    }
}