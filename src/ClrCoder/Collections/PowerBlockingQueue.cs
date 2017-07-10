// <copyright file="PowerBlockingQueue.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Validation;

    /// <summary>
    /// Power and flexible queue. Allows free enqueue and dequeue strategy with blocking.
    /// </summary>
    /// <remarks>
    /// You should queue new items to the end of the list and dequeue items from the start of the list.
    /// Using the same convention allows to multiple strategies coexists on one queue. <br/>
    /// Also inner collection should not be changed on failed enqueue/dequeue operations.
    /// </remarks>
    /// <typeparam name="TInnerCollection">The type of the inner queue collection.</typeparam>
    [PublicAPI]
    public class PowerBlockingQueue<TInnerCollection>
        where TInnerCollection : class
    {
        private readonly TInnerCollection _items;

        private readonly object _syncRoot = new object();

        private LinkedList<(TaskCompletionSource<ValueVoid>, Func<TInnerCollection, bool>)> _pendingEnqueueActions =
            new LinkedList<(TaskCompletionSource<ValueVoid>, Func<TInnerCollection, bool>)>();

        private LinkedList<(TaskCompletionSource<ValueVoid>, Func<TInnerCollection, bool>)> _pendingDequeueActions =
            new LinkedList<(TaskCompletionSource<ValueVoid>, Func<TInnerCollection, bool>)>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PowerBlockingQueue{TInnerCollection}"/> class.
        /// </summary>
        /// <param name="items">The inner queue collection.</param>
        public PowerBlockingQueue(TInnerCollection items)
        {
            VxArgs.NotNull(items, nameof(items));
            _items = items;
        }

        /// <summary>
        /// Dequeues item(s) from the queue with potential blocking.
        /// </summary>
        /// <param name="tryDequeueFunc">The try-enqueue action, called on each collection state change after dequeue operation.</param>
        public Task Dequeue([NotNull] Func<TInnerCollection, bool> tryDequeueFunc)
        {
            VxArgs.NotNull(tryDequeueFunc, nameof(tryDequeueFunc));

            lock (_syncRoot)
            {
                if (tryDequeueFunc(_items))
                {
                    TryPendingActions(false);

                    return Task.CompletedTask;
                }

                var completionSource = new TaskCompletionSource<ValueVoid>();

                _pendingDequeueActions.AddLast((completionSource, tryDequeueFunc));

                return completionSource.Task;
            }
        }

        /// <summary>
        /// Enqueues item(s) to the queue with potential blocking.
        /// </summary>
        /// <param name="tryEnqueueFunc">The try-enqueue action, called on each collection state change after dequeue operation.</param>
        public Task Enqueue([NotNull] Func<TInnerCollection, bool> tryEnqueueFunc)
        {
            VxArgs.NotNull(tryEnqueueFunc, nameof(tryEnqueueFunc));

            lock (_syncRoot)
            {
                if (tryEnqueueFunc(_items))
                {
                    TryPendingActions(true);

                    return Task.CompletedTask;
                }

                var completionSource = new TaskCompletionSource<ValueVoid>();

                _pendingEnqueueActions.AddLast((completionSource, tryEnqueueFunc));

                return completionSource.Task;
            }
        }

        private void TryPendingActions(bool tryEnqueueFirst)
        {
            bool hasChanges;
            var firstIteration = true;
            do
            {
                hasChanges = false;
                if (firstIteration && tryEnqueueFirst)
                {
                    bool hasEnqueueChanges;
                    do
                    {
                        hasEnqueueChanges = false;
                        LinkedListNode<(TaskCompletionSource<ValueVoid> Tcs, Func<TInnerCollection, bool> PendingAction)> node = _pendingEnqueueActions.First;
                        while (node != null)
                        {
                            if (node.Value.PendingAction(_items))
                            {
                                hasEnqueueChanges = true;
                                hasChanges = true;
                                node.Value.Tcs.SetResult(default(ValueVoid));
                                LinkedListNode<(TaskCompletionSource<ValueVoid> Tcs, Func<TInnerCollection, bool> PendingAction)> nodeToDelete = node;
                                node = node.Next;
                                _pendingEnqueueActions.Remove(nodeToDelete);
                            }
                            else
                            {
                                node = node.Next;
                            }
                        }
                    }
                    while (hasEnqueueChanges);
                }

                bool hasDequeueChanges;
                do
                {
                    hasDequeueChanges = false;
                    LinkedListNode<(TaskCompletionSource<ValueVoid> Tcs, Func<TInnerCollection, bool> PendingAction)> node = _pendingDequeueActions.First;
                    while (node != null)
                    {
                        if (node.Value.PendingAction(_items))
                        {
                            hasChanges = true;
                            hasDequeueChanges = true;
                            node.Value.Tcs.SetResult(default(ValueVoid));
                            LinkedListNode<(TaskCompletionSource<ValueVoid> Tcs, Func<TInnerCollection, bool> PendingAction)> nodeToDelete = node;
                            node = node.Next;
                            _pendingDequeueActions.Remove(nodeToDelete);
                        }
                        else
                        {
                            node = node.Next;
                        }
                    }
                }
                while (hasDequeueChanges);

                firstIteration = false;
            }
            while (hasChanges);
        }
    }
}