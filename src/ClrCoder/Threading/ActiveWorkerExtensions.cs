// <copyright file="ActiveWorkerExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods related to the <see cref="ActiveWorker"/> class.
    /// </summary>
    public static class ActiveWorkerExtensions
    {
        /// <summary>
        /// TODO: Document me.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="workItem"></param>
        /// <returns></returns>
        public static async Task<T> WithBlocker<T>(this IActiveWorkItem workItem, Func<Task<T>> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            using (workItem.EnterWorkBlocker())
            {
                return await action();
            }
        }
    }
}