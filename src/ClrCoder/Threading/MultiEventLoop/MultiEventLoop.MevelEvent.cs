// <copyright file="MultiEventLoop.MevelEvent.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

#if NETSTANDARD1_3 || NETSTANDARD1_6 || NETSTANDARD2_0
namespace ClrCoder.Threading
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <content>The <see cref="MevelEvent"/> <see langword="struct"/> definition.</content>
    public static partial class MultiEventLoop
    {
        private struct MevelEvent
        {
            [CanBeNull]
            public Action SimpleAction;

            [CanBeNull]
            public Action<object> Action;

            [CanBeNull]
            public object State;

            [CanBeNull]
            public Task Task;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void ExecuteEventAndClear()
            {
                if (SimpleAction != null)
                {
                    var simpleAction = SimpleAction;
                    SimpleAction = null;

                    simpleAction();
                }
                else if (Action != null)
                {
                    var action = Action;
                    Action = null;

                    var state = State;
                    State = null;

                    // ReSharper disable once PossibleNullReferenceException
                    action(state);
                }
                else
                {
                    Debug.Assert(Task != null, "Task != null");
                    var task = Task;
                    Task = null;

                    // ReSharper disable once PossibleNullReferenceException
                    // ReSharper disable once AssignNullToNotNullAttribute
                    _scheduler.TryExecuteTaskInternal(task);
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void ExecuteEvent()
            {
                if (SimpleAction != null)
                {
                    SimpleAction();
                }
                else if (Action != null)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    Action(State);
                }
                else
                {
                    Debug.Assert(Task != null, "Task != null");

                    // ReSharper disable once PossibleNullReferenceException
                    // ReSharper disable once AssignNullToNotNullAttribute
                    _scheduler.TryExecuteTaskInternal(Task);
                }
            }

            public void SetAction(Action<object> action, object state)
            {
                Action = action;
                State = state;
            }
        }
    }
}

#endif