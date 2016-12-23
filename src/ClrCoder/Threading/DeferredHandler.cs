// <copyright file="DeferredHandler.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    using System;
    using System.Collections.Concurrent;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

#if NET46
    using System.Threading;
#endif

    /// <summary>
    /// <para>Standard implementation of deferred handler. TODO: Optimize me. <br/></para>
    /// <para>Current implementation have additional memory copying and at least 2 object createion per call.</para>
    /// </summary>
    [PublicAPI]
    public class DeferredHandler
    {
        private readonly Task _backgroundWorker;

        private readonly BlockingCollection<Action> _deferredQueue = new BlockingCollection<Action>();

        //// ReSharper disable once RedundantDefaultMemberInitializer
        private bool _deferThreadAborted = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeferredHandler"/> class.
        /// </summary>
        public DeferredHandler()
        {
            _backgroundWorker = Task.Factory.StartNew(BackgroundWorkerProc, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// <see cref="DeferredHandler.Defer(System.Action)"/> <paramref name="action"/> without arguments.
        /// </summary>
        /// <param name="action">Action to execute in a background.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Defer(Action action)
        {
            if (_deferThreadAborted)
            {
                action();
            }
            else
            {
                _deferredQueue.Add(action);
            }
        }

        /// <summary>
        /// <see cref="DeferredHandler.Defer(System.Action)"/> <paramref name="action"/> with one argument.
        /// </summary>
        /// <typeparam name="T">Type of argument 1.</typeparam>
        /// <param name="action">Action to execute.</param>
        /// <param name="arg">Argument 1.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Defer<T>(Action<T> action, T arg)
        {
            if (_deferThreadAborted)
            {
                action(arg);
            }
            else
            {
                _deferredQueue.Add(() => { action(arg); });
            }
        }

        /// <summary>
        /// <see cref="DeferredHandler.Defer(System.Action)"/> <paramref name="action"/> with two argument.
        /// </summary>
        /// <typeparam name="T1">Type of argument 1.</typeparam>
        /// <typeparam name="T2">Type of argument 2.</typeparam>
        /// <param name="action">Action to execute.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Defer<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            if (_deferThreadAborted)
            {
                action(arg1, arg2);
            }
            else
            {
                _deferredQueue.Add(() => { action(arg1, arg2); });
            }
        }

        /// <summary>
        /// <see cref="DeferredHandler.Defer(System.Action)"/> <paramref name="action"/> with three argument.
        /// </summary>
        /// <typeparam name="T1">Type of argument 1.</typeparam>
        /// <typeparam name="T2">Type of argument 2.</typeparam>
        /// <typeparam name="T3">Type of argument 3.</typeparam>
        /// <param name="action">Action to execute.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Defer<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
        {
            if (_deferThreadAborted)
            {
                action(arg1, arg2, arg3);
            }
            else
            {
                _deferredQueue.Add(() => { action(arg1, arg2, arg3); });
            }
        }

        /// <summary>
        /// <see cref="DeferredHandler.Defer(System.Action)"/> <paramref name="action"/> with 4 argument.
        /// </summary>
        /// <typeparam name="T1">Type of argument 1.</typeparam>
        /// <typeparam name="T2">Type of argument 2.</typeparam>
        /// <typeparam name="T3">Type of argument 3.</typeparam>
        /// <typeparam name="T4">Type of argument 4.</typeparam>
        /// <param name="action">Action to execute.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <param name="arg4">Argument 4.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Defer<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (_deferThreadAborted)
            {
                action(arg1, arg2, arg3, arg4);
            }
            else
            {
                _deferredQueue.Add(() => { action(arg1, arg2, arg3, arg4); });
            }
        }

        /// <summary>
        /// <see cref="DeferredHandler.Defer(System.Action)"/> <paramref name="action"/> with 5 argument.
        /// </summary>
        /// <typeparam name="T1">Type of argument 1.</typeparam>
        /// <typeparam name="T2">Type of argument 2.</typeparam>
        /// <typeparam name="T3">Type of argument 3.</typeparam>
        /// <typeparam name="T4">Type of argument 4.</typeparam>
        /// <typeparam name="T5">Type of argument 5.</typeparam>
        /// <param name="action">Action to execute.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <param name="arg4">Argument 4.</param>
        /// <param name="arg5">Argument 5.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Defer<T1, T2, T3, T4, T5>(
            Action<T1, T2, T3, T4, T5> action,
            T1 arg1,
            T2 arg2,
            T3 arg3,
            T4 arg4,
            T5 arg5)
        {
            if (_deferThreadAborted)
            {
                action(arg1, arg2, arg3, arg4, arg5);
            }
            else
            {
                _deferredQueue.Add(() => { action(arg1, arg2, arg3, arg4, arg5); });
            }
        }

        /// <summary>
        /// <see cref="DeferredHandler.Defer(System.Action)"/> <paramref name="action"/> with 6 argument.
        /// </summary>
        /// <typeparam name="T1">Type of argument 1.</typeparam>
        /// <typeparam name="T2">Type of argument 2.</typeparam>
        /// <typeparam name="T3">Type of argument 3.</typeparam>
        /// <typeparam name="T4">Type of argument 4.</typeparam>
        /// <typeparam name="T5">Type of argument 5.</typeparam>
        /// <typeparam name="T6">Type of argument 6.</typeparam>
        /// <param name="action">Action to execute.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <param name="arg4">Argument 4.</param>
        /// <param name="arg5">Argument 5.</param>
        /// <param name="arg6">Argument 6.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Defer<T1, T2, T3, T4, T5, T6>(
            Action<T1, T2, T3, T4, T5, T6> action,
            T1 arg1,
            T2 arg2,
            T3 arg3,
            T4 arg4,
            T5 arg5,
            T6 arg6)
        {
            if (_deferThreadAborted)
            {
                action(arg1, arg2, arg3, arg4, arg5, arg6);
            }
            else
            {
                _deferredQueue.Add(() => { action(arg1, arg2, arg3, arg4, arg5, arg6); });
            }
        }

        /// <summary>
        /// <see cref="DeferredHandler.Defer(System.Action)"/> <paramref name="action"/> with 7 argument.
        /// </summary>
        /// <typeparam name="T1">Type of argument 1.</typeparam>
        /// <typeparam name="T2">Type of argument 2.</typeparam>
        /// <typeparam name="T3">Type of argument 3.</typeparam>
        /// <typeparam name="T4">Type of argument 4.</typeparam>
        /// <typeparam name="T5">Type of argument 5.</typeparam>
        /// <typeparam name="T6">Type of argument 6.</typeparam>
        /// <typeparam name="T7">Type of argument 7.</typeparam>
        /// <param name="action">Action to execute.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <param name="arg4">Argument 4.</param>
        /// <param name="arg5">Argument 5.</param>
        /// <param name="arg6">Argument 6.</param>
        /// <param name="arg7">Argument 7.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Defer<T1, T2, T3, T4, T5, T6, T7>(
            Action<T1, T2, T3, T4, T5, T6, T7> action,
            T1 arg1,
            T2 arg2,
            T3 arg3,
            T4 arg4,
            T5 arg5,
            T6 arg6,
            T7 arg7)
        {
            if (_deferThreadAborted)
            {
                action(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            }
            else
            {
                _deferredQueue.Add(() => { action(arg1, arg2, arg3, arg4, arg5, arg6, arg7); });
            }
        }

        /// <summary>
        /// <see cref="DeferredHandler.Defer(System.Action)"/> <paramref name="action"/> with 8 argument.
        /// </summary>
        /// <typeparam name="T1">Type of argument 1.</typeparam>
        /// <typeparam name="T2">Type of argument 2.</typeparam>
        /// <typeparam name="T3">Type of argument 3.</typeparam>
        /// <typeparam name="T4">Type of argument 4.</typeparam>
        /// <typeparam name="T5">Type of argument 5.</typeparam>
        /// <typeparam name="T6">Type of argument 6.</typeparam>
        /// <typeparam name="T7">Type of argument 7.</typeparam>
        /// <typeparam name="T8">Type of argument 8.</typeparam>
        /// <param name="action">Action to execute.</param>
        /// <param name="arg1">Argument 1.</param>
        /// <param name="arg2">Argument 2.</param>
        /// <param name="arg3">Argument 3.</param>
        /// <param name="arg4">Argument 4.</param>
        /// <param name="arg5">Argument 5.</param>
        /// <param name="arg6">Argument 6.</param>
        /// <param name="arg7">Argument 7.</param>
        /// <param name="arg8">Argument 8.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Defer<T1, T2, T3, T4, T5, T6, T7, T8>(
            Action<T1, T2, T3, T4, T5, T6, T7, T8> action,
            T1 arg1,
            T2 arg2,
            T3 arg3,
            T4 arg4,
            T5 arg5,
            T6 arg6,
            T7 arg7,
            T8 arg8)
        {
            if (_deferThreadAborted)
            {
                action(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            }
            else
            {
                _deferredQueue.Add(() => { action(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8); });
            }
        }

        private void BackgroundWorkerProc()
        {
            try
            {
                for (;;)
                {
                    var deferredAction = _deferredQueue.Take();
                    try
                    {
                        deferredAction();
                    }
                    catch (Exception ex)
                    {
                        // TODO: Hack
                        Console.WriteLine(ex);
                    }
                }
            }
#if NET46
            catch (ThreadAbortException)
            {
                _deferThreadAborted = true;
                Action action;
                while (_deferredQueue.TryTake(out action, TimeSpan.FromMilliseconds(200)))
                {
                    try
                    {
                        action();
                    }
                    catch (Exception)
                    {
                        // Mute errors.
                    }
                }
            }
#endif
            catch (Exception)
            {
                // Mute errors.
            }
        }
    }
}