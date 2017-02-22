// <copyright file="DelayedEventSource.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Produces event after a specified timeout.
    /// </summary>
    public class DelayedEventSource
    {
        private readonly Action _action;

        private readonly CancellationToken _cancellationToken;

        private int _actionId;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelayedEventSource"/> class.
        /// </summary>
        /// <param name="action">Event handler.</param>
        /// <param name="cancellationToken">Cancels event rising.</param>
        public DelayedEventSource(Action action, CancellationToken cancellationToken = default(CancellationToken))
        {
            _action = action;
            _cancellationToken = cancellationToken;
        }

        /// <summary>
        /// Sets new timeout to execute task. This method always overrides previously set timeout.<br/>
        /// After event is raised event source do nothing until next call to SetNextTimeout.
        /// </summary>
        /// <param name="delay">Timeout delay from current instance before rise event.</param>
        public void SetNextTimeout(TimeSpan delay)
        {
            if (!_cancellationToken.IsCancellationRequested)
            {
                int id = Interlocked.Increment(ref _actionId);
                if (delay != Timeout.InfiniteTimeSpan)
                {
                    DoActionAfterDelay(delay, id);
                }
            }
        }

        private async void DoActionAfterDelay(TimeSpan delay, int id)
        {
            try
            {
                Interlocked.MemoryBarrier();
                if (_actionId == id)
                {
                    await Task.Delay(delay, _cancellationToken);
                    _action();
                }
            }
            catch (OperationCanceledException)
            {
                // Do nothing.
            }
            catch (Exception)
            {
                // Do nothing.
            }
        }
    }
}