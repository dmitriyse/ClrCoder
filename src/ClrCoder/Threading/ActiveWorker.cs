// <copyright file="ActiveWorker.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Active worker. Allows to actively (continuously) produce work items, until some some saturation reached.
    /// </summary>
    public class ActiveWorker : AsyncDisposableBase
    {
        private readonly Func<ActiveWorkItem, Task> _workAction;

        private readonly object _blockerSectionSyncRoot = new object();

        private readonly CancellationTokenSource _cts;

        private readonly DelayedEventSource _scheduleNewWorkItemEventSource;

        private readonly TimeSpan _gracePeriod;

        private readonly HashSet<ActiveWorkItem> _activeWorkItems = new HashSet<ActiveWorkItem>();

        private readonly HashSet<BlockerSectionLock> _enteredBlockedSections = new HashSet<BlockerSectionLock>();

        private CancellationTokenSource _workTerminatedCts;

        private CancellationToken _workTerminatedToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveWorker"/> class.
        /// </summary>
        /// <param name="workAction">Work item handler</param>
        /// <param name="newWorkItemGracePeriod">Grace period before new work item creation. 100ms if nothing is specified.</param>
        public ActiveWorker(Func<IActiveWorkItem, Task> workAction, TimeSpan? newWorkItemGracePeriod = null)
        {
            if (workAction == null)
            {
                throw new ArgumentNullException(nameof(workAction));
            }

            if (newWorkItemGracePeriod == null)
            {
                newWorkItemGracePeriod = TimeSpan.FromMilliseconds(100);
            }

            if (newWorkItemGracePeriod <= TimeSpan.Zero || newWorkItemGracePeriod == Timeout.InfiniteTimeSpan)
            {
                throw new ArgumentException(
                    "Grace period should be positive and not infinite.",
                    nameof(newWorkItemGracePeriod));
            }

            _workAction = workAction;
            _cts = new CancellationTokenSource();
            _gracePeriod = newWorkItemGracePeriod.Value;
            _scheduleNewWorkItemEventSource = new DelayedEventSource(CreateNewWorkItem, _cts.Token);

            // Emitting first work item.
            _scheduleNewWorkItemEventSource.SetNextTimeout(_gracePeriod);
        }

        private bool WorkTerminationStarted => _workTerminatedCts != null;

        /// <inheritdoc/>
        protected override async Task DisposeAsyncCore()
        {
            lock (_blockerSectionSyncRoot)
            {
                _cts.Cancel();

                _workTerminatedCts = new CancellationTokenSource();
                _workTerminatedToken = _workTerminatedCts.Token;
                if (!_activeWorkItems.Any())
                {
                    _workTerminatedCts.Cancel();
                }
            }

            await _workTerminatedToken;

            _workTerminatedCts.Dispose();
            _cts.Dispose();
        }

        private async void CreateNewWorkItem()
        {
            // We can be here even after _cts.Cancel() or after WorkTerminationStarted becomes true.
            // This occurs where delayed event raises in near the same time with _cts.Cancel() call.
            // ==================================================================================================
            ActiveWorkItem workItem = null;

            lock (_blockerSectionSyncRoot)
            {
                if (!_enteredBlockedSections.Any() && !WorkTerminationStarted)
                {
                    _scheduleNewWorkItemEventSource.SetNextTimeout(_gracePeriod);
                    workItem = new ActiveWorkItem(this);
                    _activeWorkItems.Add(workItem);
                }
            }

            if (workItem != null)
            {
                try
                {
                    // Running new work item.
                    await _workAction(workItem);
                }
                catch (Exception)
                {
                    // Do nothing.
                }
                finally
                {
                    lock (_blockerSectionSyncRoot)
                    {
                        bool removed = _activeWorkItems.Remove(workItem);
                        Debug.Assert(removed, "Double remove is wrong");

                        // When no more work items left and termination requested - setting termination finish cts event.
                        if (WorkTerminationStarted && !_activeWorkItems.Any())
                        {
                            // Synchronous continuation impossible here, so no any reentrancy!
                            _workTerminatedCts.Cancel();
                        }

                        // Raising critical errors if some locks was not released when task exited.
                        foreach (BlockerSectionLock sectionLock in workItem.Locks)
                        {
                            Critical.Assert(
                                false,
                                $"Unreleased work blocker section at {sectionLock.CallerInfo.FilePath}:{sectionLock.CallerInfo.LineNumber} in {sectionLock.CallerInfo.MemberName}");
                        }
                    }
                }
            }
        }

        private IDisposable EnterWorkBlockSection(ActiveWorkItem workItem, string debugInfo, CallerInfo callerInfo)
        {
            lock (_blockerSectionSyncRoot)
            {
                var sectionLock = new BlockerSectionLock(workItem, callerInfo);

                // Registering lock.
                _enteredBlockedSections.Add(sectionLock);
                sectionLock.WorkItem.Locks.Add(sectionLock);

                // Disabling new work item scheduling.
                _scheduleNewWorkItemEventSource.SetNextTimeout(Timeout.InfiniteTimeSpan);

                return sectionLock;
            }
        }

        private void ExitWorkBlockSection(BlockerSectionLock sectionLock)
        {
            if (sectionLock == null)
            {
                throw new ArgumentNullException(nameof(sectionLock));
            }

            lock (_blockerSectionSyncRoot)
            {
                bool removed = sectionLock.WorkItem.Locks.Remove(sectionLock);
                Debug.Assert(removed, "Double remove error.");

                removed = _enteredBlockedSections.Remove(sectionLock);
                Debug.Assert(removed, "Double remove error.");

                // Scheduling next work item after some grace period.
                if (!_enteredBlockedSections.Any() && !WorkTerminationStarted)
                {
                    _scheduleNewWorkItemEventSource.SetNextTimeout(_gracePeriod);
                }
            }
        }

        private class ActiveWorkItem : IActiveWorkItem
        {
            public ActiveWorkItem(ActiveWorker owner)
            {
                Owner = owner;
            }

            public ActiveWorker Owner { get; }

            public HashSet<BlockerSectionLock> Locks { get; } = new HashSet<BlockerSectionLock>();

            /// <inheritdoc/>
            public IDisposable EnterWorkBlocker(
                string debugInfo = "",
                [CallerFilePath] string filePath = null,
                [CallerMemberName] string memberName = null,
                [CallerLineNumber] int lineNumber = 0)
            {
                // ReSharper disable AssignNullToNotNullAttribute
                return Owner.EnterWorkBlockSection(this, debugInfo, new CallerInfo(filePath, memberName, lineNumber));

                // ReSharper restore AssignNullToNotNullAttribute
            }
        }

        private class BlockerSectionLock : IDisposable
        {
            public BlockerSectionLock(ActiveWorkItem workItem, CallerInfo callerInfo)
            {
                if (workItem == null)
                {
                    throw new ArgumentNullException(nameof(workItem));
                }

                WorkItem = workItem;
                CallerInfo = callerInfo;
            }

            public ActiveWorkItem WorkItem { get; }

            public CallerInfo CallerInfo { get; }

            /// <inheritdoc/>
            public void Dispose()
            {
                WorkItem.Owner.ExitWorkBlockSection(this);
            }
        }
    }
}