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

    using JetBrains.Annotations;

    /// <summary>
    /// Active worker. Allows to actively (continuously) produce work items, until some some saturation reached.
    /// </summary>
    [PublicAPI]
    public class ActiveWorker : AsyncDisposableBase
    {
        private readonly object _workBlockersSyncRoot = new object();

        private readonly HashSet<ActiveWorkItem> _activeWorkItems = new HashSet<ActiveWorkItem>();

        private readonly HashSet<WorkBlocker> _workBlockers = new HashSet<WorkBlocker>();

        private readonly ActiveWorkerWorkItemAction _workItemAction;

        private readonly TimeSpan _gracePeriod;

        private readonly Task _workItemsGeneratorThread;

        private readonly CancellationTokenSource _shutdownCts;

        private readonly CancellationTokenSource _shutdownCompletedCts;

        private ManualResetEventSlim _noMoreBlockersEvent = new ManualResetEventSlim();

        private CancellationToken _shutdownCancellationToken;

        private CancellationToken _shutdownCompletedCancellationToken;

        private long _workItemValidationId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveWorker"/> class.
        /// </summary>
        /// <param name="workItemAction">The work item action.</param>
        /// <param name="newWorkItemGracePeriod">Grace period before new work item creation. 100ms if nothing is specified.</param>
        public ActiveWorker(
            ActiveWorkerWorkItemAction workItemAction,
            TimeSpan newWorkItemGracePeriod = default(TimeSpan))
        {
            if (workItemAction == null)
            {
                throw new ArgumentNullException(nameof(workItemAction));
            }

            if ((newWorkItemGracePeriod < TimeSpan.Zero) || (newWorkItemGracePeriod == Timeout.InfiniteTimeSpan))
            {
                throw new ArgumentException(
                    "The grace period should not be negative and or infinite.",
                    nameof(newWorkItemGracePeriod));
            }

            _workItemAction = workItemAction;
            _gracePeriod = newWorkItemGracePeriod;

            _shutdownCts = new CancellationTokenSource();
            _shutdownCancellationToken = _shutdownCts.Token;

            _shutdownCompletedCts = new CancellationTokenSource();
            _shutdownCompletedCancellationToken = _shutdownCompletedCts.Token;

            _workItemsGeneratorThread = Task.Factory.StartNew(
                WorkItemsGeneratorThreadProc,
                TaskCreationOptions.LongRunning);

            // Hey! Starting the show.
            _noMoreBlockersEvent.Set();
        }

        private bool IsShutdownStarted => _shutdownCancellationToken.IsCancellationRequested;

        private static string CallerInfoToString(string debugInfo, CallerInfo callerInfo)
        {
            var str = $"{callerInfo.FilePath}:{callerInfo.LineNumber} in {callerInfo.MemberName}.";

            if (!string.IsNullOrWhiteSpace(debugInfo))
            {
                str += $" Info: {debugInfo}";
            }

            return str;
        }

        /// <inheritdoc/>
        protected override async Task DisposeAsyncCore()
        {
            // Starting shutdown procedure.
            lock (_workBlockersSyncRoot)
            {
                // Canceling currently is being created work item.
                _workItemValidationId++;

                // Signaling shutdown start.
                _shutdownCts.Cancel();

                // If there are no any running work items, signaling shutdown completed.
                // Otherwise nobody would signal shutdown completed.
                if (!_activeWorkItems.Any())
                {
                    _shutdownCompletedCts.Cancel();
                }
            }

            try
            {
                await _workItemsGeneratorThread;
            }
            catch
            {
                // Do nothing.
            }

            await _shutdownCompletedCancellationToken;

            _shutdownCompletedCts.Dispose();
            _shutdownCts.Dispose();
        }

        private WorkBlocker EnterWorkBlocker(ActiveWorkItem workItem, string debugInfo, CallerInfo callerInfo)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            Debug.Assert(workItem != null, "workItem != null");

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            Debug.Assert(debugInfo != null, "debugInfo != null");

            lock (_workBlockersSyncRoot)
            {
                // Validating work item is active.
                if (!_activeWorkItems.Contains(workItem))
                {
                    throw new InvalidOperationException(
                        $"Error entering work blocker section. Work item already completed. {CallerInfoToString(debugInfo, callerInfo)}");
                }

                // !!! Everything has changed, after we have captured the lock. !!!
                ////
                // Canceling currently is being created work item.
                _workItemValidationId++;

                var workBlocker = new WorkBlocker(false, workItem, debugInfo, callerInfo);

                // Registering blocker.
                _workBlockers.Add(workBlocker);
                workBlocker.WorkItem.Blockers.Add(workBlocker);

                return workBlocker;
            }
        }

        private async Task ExecuteNewWorkItem(long workItemValidationId)
        {
            ActiveWorkItem workItem;
            WorkBlocker workBlocker;

            lock (_workBlockersSyncRoot)
            {
                // !!! Everything has changed in the world, after we are captured the lock. !!!
                ////
                _shutdownCancellationToken.ThrowIfCancellationRequested();

                // Do nothing, if work item creation has been canceled.
                if (workItemValidationId != _workItemValidationId)
                {
                    return;
                }

                // Creating and registering a new work item.
                workItem = new ActiveWorkItem(this);

                // Creating initial work blocker for the new work item.
                workBlocker = new WorkBlocker(
                    true,
                    workItem,
                    string.Empty,
                    new CallerInfo("new work item", "initial_blocker", 0));

                // Registering blocker.
                _workBlockers.Add(workBlocker);
                workItem.Blockers.Add(workBlocker);

                _activeWorkItems.Add(workItem);
            }

            try
            {
                // Running new work item.
                await _workItemAction(workItem, new ActiveWorkerBlockerToken(workBlocker));
            }
            finally
            {
                lock (_workBlockersSyncRoot)
                {
                    bool removed = _activeWorkItems.Remove(workItem);
                    Debug.Assert(removed, "Double remove is wrong");

                    // When no more work items left and termination requested - setting termination finish cts event.
                    if (IsShutdownStarted && !_activeWorkItems.Any())
                    {
                        _shutdownCompletedCts.Cancel();
                    }

                    // It's allowed to not dispose initial blocker.
                    if ((workItem.Blockers.Count == 1) && workItem.Blockers.First().IsInitialBlocker)
                    {
                        // Disposing last initial blocker.
                        workItem.Blockers.Remove(workItem.Blockers.First());
                        _workBlockers.Remove(workItem.Blockers.First());
                    }
                    else
                    {
                        // Raising critical errors if some locks was not released when task exited.
                        foreach (WorkBlocker unreleasedBlockers in workItem.Blockers)
                        {
                            Critical.Assert(
                                false,
                                $"Unreleased work blocker section at {CallerInfoToString(unreleasedBlockers.DebugInfo, unreleasedBlockers.CallerInfo)}");
                        }
                    }
                }
            }
        }

        private void ExitWorkBlocker(WorkBlocker workBlocker)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            Debug.Assert(workBlocker != null, "workBlocker != null");

            lock (_workBlockersSyncRoot)
            {
                // !!! Everything has changed, after we have captured the lock. !!!
                ////
                bool removed = workBlocker.WorkItem.Blockers.Remove(workBlocker);
                if (!removed)
                {
                    throw new InvalidOperationException(
                        $"Error exiting work blocker section. Work blocker already released. {CallerInfoToString(workBlocker.DebugInfo, workBlocker.CallerInfo)}");
                }

                removed = _workBlockers.Remove(workBlocker);
                Debug.Assert(removed, "Double remove error.");

                // Scheduling next work item after some grace period.
                if (!_workBlockers.Any() && !IsShutdownStarted)
                {
                    // Raising event for starting new work item.
                    _noMoreBlockersEvent.Set();
                }
            }
        }

        private void WorkItemsGeneratorThreadProc()
        {
            while (true)
            {
                try
                {
                    _noMoreBlockersEvent.Wait(_shutdownCancellationToken);

                    // Some lock/unlock actions can be performed here.
                    ////
                    lock (_workBlockersSyncRoot)
                    {
                        // !!! Everything has changed in the world, after we have captured the lock. !!!
                        ////
                        // Do nothing on shutdown.
                        _shutdownCancellationToken.ThrowIfCancellationRequested();

                        // We needs to reset event to listen signals on the next iteration.
                        _noMoreBlockersEvent.Reset();

                        // Only if there are no any blockers, starting new work item.
                        if (!_workBlockers.Any())
                        {
                            // Issuing brand new validation id :)
                            var workItemValidationId = ++_workItemValidationId;

                            // Fire and forget.
                            // ReSharper disable once MethodSupportsCancellation
                            Task.Run(
                                async () =>
                                    {
                                        try
                                        {
                                            // Everything has changed in the world, after we have passed in the new task.
                                            ////
                                            if (_gracePeriod != TimeSpan.Zero)
                                            {
                                                await Task.Delay(_gracePeriod, _shutdownCancellationToken);
                                            }

                                            _shutdownCancellationToken.ThrowIfCancellationRequested();

                                            await ExecuteNewWorkItem(workItemValidationId);
                                        }
                                        catch (OperationCanceledException)
                                        {
                                            // Do nothing, it's a valid exit way.
                                        }
                                        catch
                                        {
                                            // Do nothing, even on any error.
                                        }
                                    });
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// The work item.
        /// </summary>
        internal class ActiveWorkItem : IActiveWorkItem
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ActiveWorkItem"/> class.
            /// </summary>
            /// <param name="owner">The owner active worker.</param>
            public ActiveWorkItem(ActiveWorker owner)
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                Debug.Assert(owner != null, "owner != null");

                Owner = owner;
            }

            /// <summary>
            /// The owner active worker.
            /// </summary>
            public ActiveWorker Owner { get; }

            /// <summary>
            /// The collection of work blockers associated with the work item.
            /// </summary>
            public HashSet<WorkBlocker> Blockers { get; } = new HashSet<WorkBlocker>();

            /// <inheritdoc/>
            public ActiveWorkerBlockerToken EnterWorkBlocker(
                [CanBeNull] string debugInfo = "",
                [CallerFilePath] string filePath = null,
                [CallerMemberName] string memberName = null,
                [CallerLineNumber] int lineNumber = 0)
            {
                debugInfo = debugInfo ?? string.Empty;

                // ReSharper disable AssignNullToNotNullAttribute
                return new ActiveWorkerBlockerToken(
                    Owner.EnterWorkBlocker(this, debugInfo, new CallerInfo(filePath, memberName, lineNumber)));

                // ReSharper restore AssignNullToNotNullAttribute
            }
        }

        /// <summary>
        /// The work blocker. Can be replaced to reference counting logic in the future to improve performance.
        /// </summary>
        internal class WorkBlocker : IDisposable
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="WorkBlocker"/> class.
            /// </summary>
            /// <param name="isInitialBlocker">Shows that this is the initial blocker.</param>
            /// <param name="workItem">The owner work item.</param>
            /// <param name="debugInfo">The additional debug information bound to the work blocker section entry point.</param>
            /// <param name="callerInfo">The caller info of the work blocker section entry point.</param>
            public WorkBlocker(bool isInitialBlocker, ActiveWorkItem workItem, string debugInfo, CallerInfo callerInfo)
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                Debug.Assert(workItem != null, "workItem != null");

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                Debug.Assert(debugInfo != null, "debugInfo != null");

                IsInitialBlocker = isInitialBlocker;

                WorkItem = workItem;
                DebugInfo = debugInfo;
                CallerInfo = callerInfo;
            }

            /// <summary>
            /// Shows that this is the initial blocker.
            /// </summary>
            public bool IsInitialBlocker { get; }

            /// <summary>
            /// The owner work item.
            /// </summary>
            public ActiveWorkItem WorkItem { get; }

            /// <summary>
            /// The additional debug information bound to the work blocker section entry point.
            /// </summary>
            public string DebugInfo { get; }

            /// <summary>
            /// The caller info of the work blocker section entry point.
            /// </summary>
            public CallerInfo CallerInfo { get; }

            /// <inheritdoc/>
            public void Dispose()
            {
                WorkItem.Owner.ExitWorkBlocker(this);
            }
        }
    }

    /// <summary>
    /// The <see cref="ActiveWorker"/> work item action.
    /// </summary>
    /// <param name="workItem">The work item.</param>
    /// <param name="initialBlocker">The initial blocker, should be disposed to allow other work items creation.</param>
    /// <returns>The async execution TPL task.</returns>
    public delegate Task ActiveWorkerWorkItemAction(IActiveWorkItem workItem, ActiveWorkerBlockerToken initialBlocker);
}