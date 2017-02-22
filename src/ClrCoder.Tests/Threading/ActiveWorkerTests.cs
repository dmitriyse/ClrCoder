// <copyright file="ActiveWorkerTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests.Threading
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using ClrCoder.Threading;

    using FluentAssertions;

    using NUnit.Framework;

    /// <summary>
    /// Tests for the <see cref="ActiveWorker"/> class.
    /// </summary>
    [TestFixture]
    public class ActiveWorkerTests
    {
        /// <summary>
        /// Simple test.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        [Test]
        public async Task SimpleTest()
        {
            var allWorkItems = new HashSet<IActiveWorkItem>();
            TextWriter writer = TestContext.Out;
            var activeWorker = new ActiveWorker(
                async workItem =>
                    {
                        lock (allWorkItems)
                        {
                            allWorkItems.Add(workItem);
                        }

                        // Simple expected processing schedule.
                        //// ====================>t
                        //// ~~---XX 
                        ////    ---XX~~
                        ////           ---XX
                        ////            ---XX~~
                        //// =====================>t
                        // Estimated speed is 2 task in 350ms
                        await Task.Delay(150);

                        using (workItem.EnterWorkBlocker())
                        {
                            await Task.Delay(100);
                        }
                    });

            var delayMilliseconds = 5000;
            await activeWorker.AsyncUsing(
                async w => { await Task.Delay(delayMilliseconds); });

            int expectedAsymptotically = (int)(((delayMilliseconds - 100) * 2) / 350.0);
            int maxDelta = 3;
            writer.WriteLine($"Total executed work items = {allWorkItems.Count}");
            writer.WriteLine($"Expected asymptotically = {expectedAsymptotically}");
            Math.Abs(allWorkItems.Count - expectedAsymptotically).Should().BeLessOrEqualTo(maxDelta);
        }
    }
}