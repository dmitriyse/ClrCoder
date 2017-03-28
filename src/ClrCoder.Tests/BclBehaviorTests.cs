// <copyright file="BclBehaviorTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using FluentAssertions;

    using NUnit.Framework;

    /// <summary>
    /// Identifies behavior of BCL parts.
    /// </summary>
    [TestFixture]
    public class BclBehaviorTests
    {
        /// <summary>
        /// Async functions wraps exception into Task when exception raised in synchronous execution part.
        /// </summary>
        [Test]
        public void Async_function_exception_should_be_wrapped_in_a_sync_execution_path()
        {
            Action a = () =>
                {
                    Func<Task> someAsync = async () =>
                        {
                            await Task.CompletedTask;
                            throw new Exception("DummyError");
                        };

                    Task task = someAsync();
                    task.Should().NotBeNull();
                    task.Wait();
                };
            a.ShouldThrow<AggregateException>().WithInnerException<Exception>().WithInnerMessage("DummyError");
        }

        /// <summary>
        /// Await behavior test for completed task.
        /// </summary>
        /// <returns>Async execution task.</returns>
        [Test]
        public async Task Completed_task_should_be_awaited_synchronously()
        {
            int originalThreadId = Thread.CurrentThread.ManagedThreadId;
            await Task.CompletedTask;

            Thread.CurrentThread.ManagedThreadId.Should().Be(originalThreadId);
        }

        /// <summary>
        /// Await behavior test for completed task with ConfigureAwait(false).
        /// </summary>
        /// <returns>Async execution task.</returns>
        [Test]
        public async Task Completed_task_should_be_awaited_synchronously_even_with_configure_await_false()
        {
            int originalThreadId = Thread.CurrentThread.ManagedThreadId;
            await Task.CompletedTask.ConfigureAwait(false);

            Thread.CurrentThread.ManagedThreadId.Should().Be(originalThreadId);
        }

        /// <summary>
        /// Longest directory creation test. Result is 255 (Windows without <c>long</c> file names feature).
        /// </summary>
        /// <param name="dirNameLength">Test directory name length.</param>
        [Test]
        [TestCase(244)]
        [TestCase(245)]
        [TestCase(246)]
        [TestCase(247)]
        [TestCase(248)]
        [TestCase(255)]
        [TestCase(256)]
        [TestCase(257)]
        [TestCase(260)]
        [Ignore("For manual testing")]
        public void LongestDirectoryCreateTest(int dirNameLength)
        {
            string rootPath = EnvironmentEx.OSFamily.HasFlag(OSFamilyTypes.Posix) ? "/" : "c:\\";
            var name = new string('t', dirNameLength);
            string dirFullName = rootPath + name;
            Directory.CreateDirectory(dirFullName);
            Directory.Delete(dirFullName);
        }

        /// <summary>
        /// Longest file creation test. Result is 255 for .Net Core, and !Surprise 197! for Net46 (Windows without <c>long</c> file
        /// names feature).
        /// </summary>
        [Test]
        [Ignore("For manual testing")]
        public void LongestFileCreateTest()
        {
            string rootPath = EnvironmentEx.OSFamily.HasFlag(OSFamilyTypes.Posix) ? "/" : "c:\\";
            for (var i = 150; i < short.MaxValue; i++)
            {
                int fileNameLength = i;
                var ext = ".txt";
                var name = new string('t', fileNameLength - ext.Length);
                string fileFullName = rootPath + name + ext;
                File.WriteAllText(name, "Hello world!");
                File.Delete(fileFullName);
                TestContext.WriteLine($"File name length = {fileNameLength} is supported.");
            }
        }

        /// <summary>
        /// Longest path creation test. Result is 32700+ .Net Core, and 247 for .Net46 (Windows without <c>long</c> file names
        /// feature).
        /// </summary>
        /// <remarks>
        /// TODO: Rewrite <c>this</c> to reusable test.
        /// </remarks>
        [Test]
        [Ignore("For manual testing")]
        public void LongestPathCreateTest()
        {
            string rootPath = EnvironmentEx.OSFamily.HasFlag(OSFamilyTypes.Posix) ? "/" : "c:\\";
#if NET46
            var dirName = new string('d', 1);
#else
            var dirName = new string('d', EnvironmentEx.MaxDirectoryNameLength);
#endif
            string dirFullName = rootPath + new string('d', 200);
            Directory.CreateDirectory(dirFullName);
            try
            {
                while (true)
                {
                    dirFullName = dirFullName + Path.DirectorySeparatorChar + dirName;
#if NET46
                    if (dirFullName.Length == 244)
                    {
                        dirFullName = dirFullName + Path.DirectorySeparatorChar + dirName + 'd';
                    }
#endif
                    Directory.CreateDirectory(dirFullName);
                    if (dirFullName.Length == 32715)
                    {
                        while (true)
                        {
                            dirFullName = dirFullName + Path.DirectorySeparatorChar + new string('d', 1);
                            Directory.CreateDirectory(dirFullName);

                            if (dirFullName.Length > 32730)
                            {
                                TestContext.WriteLine($"Path length={dirFullName.Length} is supported.");
                            }
                        }
                    }

#if NET46
                    TestContext.WriteLine($"Path length={dirFullName.Length} is supported.");
#endif
                    if (dirFullName.Length > 32715)
                    {
                        TestContext.WriteLine($"Path length={dirFullName.Length} is supported.");
                    }
                }
            }
            finally
            {
                Directory.Delete(dirFullName, true);
            }
        }

        /// <summary>
        /// Long running TPL task should use background thread. This behavior required to implement supporting background services
        /// that is terminated on application exit.
        /// </summary>
        [Test]
        public void LongRunningTaskShouldUseBackgroundThreads()
        {
            var cts = new CancellationTokenSource();
#if NET46
            Task task = Task.Factory.StartNew(
                () =>
                    {
                        Thread.CurrentThread.IsBackground.Should()
                            .BeTrue("Assuming long running task spawn background thread.");

                        // Performing wait that will be terminated.
                        try
                        {
                            cts.Token.WaitHandle.WaitOne();
                        }
                        catch (ThreadAbortException)
                        {
                            // This code block actually executed, but NUnit skips this event.
                            // Inspect this message in Debug output window.
                            TestContext.Out.WriteLine("Gracefull termination");
                        }

                        // Right after the catch runtime will rise ThreadAbortException again.
                        TestContext.Out.WriteLine("Unreachable code!");
                    },
                TaskCreationOptions.LongRunning);
            task.Wait(TimeSpan.FromSeconds(1)).Should().BeFalse();
#else
            Task task = Task.Factory.StartNew(
                () =>
                    {
                        Thread.CurrentThread.IsBackground.Should()
                            .BeTrue("Assuming long running task spawn background thread.");

                        // Performing wait that will be terminated.
                        try
                        {
                            cts.Token.WaitHandle.WaitOne();
                        }
                        catch (Exception ex)
                        {
                            // TODO: Identify exception!
                            TestContext.Out.WriteLine(ex.GetType());

                            // This code block actually executed, but NUnit skips this event.
                            // Inspect this message in Debug output window.
                            TestContext.Out.WriteLine("Gracefull termination");
                        }

                        // Right after the catch runtime will rise ThreadAbortException again.
                        TestContext.Out.WriteLine("Unreachable code!");
                    },
                TaskCreationOptions.LongRunning);
            task.Wait(TimeSpan.FromSeconds(1)).Should().BeFalse();
#endif
        }

        /// <summary>
        /// Path.Combine behavior test.
        /// </summary>
        /// <param name="a">First part.</param>
        /// <param name="b">Second part.</param>
        /// <param name="expected">Expected combined path.</param>
        [Test]
        [TestCase("c:\\test.com", "abc", "c:\\test.com\\abc")]
        public void PathCombineTest(string a, string b, string expected)
        {
            Path.Combine(a, b).Replace("/", "\\").Should().Be(expected);
        }

        /// <summary>
        /// Trace.Assert behavior test.
        /// </summary>
        [Test]
        [Ignore("For manual testing")]
        public void TraceAssert_should_produce_breakpoint()
        {
            Trace.Assert(false, "break on me");
        }

        /// <summary>
        /// Tests behavior for urls where specified default port for scheme. For example http://localhost:80.
        /// </summary>
        [Test]
        public void UriDefaultPortNumbersTest()
        {
            var uriExplicit = new Uri("http://localhost:80", UriKind.Absolute);
            var uriImplicit = new Uri("http://localhost", UriKind.Absolute);
            
            uriExplicit.Equals(uriImplicit).Should().BeTrue();
            uriExplicit.AbsoluteUri.Equals(uriImplicit.AbsoluteUri).Should().BeTrue();
            TestContext.WriteLine(uriExplicit);

            var uriWrongPort1 = new Uri("http://localhost:443", UriKind.Absolute);
            var uriWrongPort2 = new Uri("https://localhost:80", UriKind.Absolute);
            uriWrongPort1.AbsoluteUri.Should().Be("http://localhost:443/");
            uriWrongPort2.AbsoluteUri.Should().Be("https://localhost:80/");
        }
    }
}