// <copyright file="DspMicrobenchmarkTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
#if NETCOREAPP2_0 || NETCOREAPP2_1
namespace ClrCoder.Tests.Dsp
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using ClrCoder.Dsp;
    using ClrCoder.Threading;

    using NUnit.Framework;

    /// <summary>
    /// DSP framework micro benchmarks.
    /// </summary>
    [TestFixture]
    public class DspMicroBenchmarkTests
    {
        /// <summary>
        /// Tests how much fast processing calls mechanics.
        /// </summary>
        /// <remarks>
        /// Speed is comparable with 3-times interface methods calls.
        /// </remarks>
        [Test]
        public void DoProtocolBenchmark()
        {
            const int TestSize = 100000000;

            var target = new SimpleTarget();
            var processor = new SimpleProcessorBlock(target);
            var source = new SimpleSource(processor);

            // Warm-up
            source.DoOperations(1);

            Stopwatch stopwatch = Stopwatch.StartNew();

            source.DoOperations(TestSize);

            stopwatch.Stop();
            double singleCoreSpeed = (TestSize * 1000.0) / stopwatch.ElapsedMilliseconds;
            TestContext.WriteLine($"single-thread:{singleCoreSpeed / 1000_000.0:F4} millions/sec");
        }

        private class SimpleProcessorBlock : IDsProcessorBlock<byte>
        {
            private readonly IDsTarget<int> _target;

            private readonly SimpleDirectProcessor _op;

            public SimpleProcessorBlock(IDsTarget<int> target)
            {
                _target = target;
                _op = new SimpleDirectProcessor(this);
            }

            public Task Process(ReadOnlySpan<byte> p1, ReadOnlySpan<byte> p2, out int processed)
            {
                return _target.ProcessDirect(_op, p1, p2, out processed);
            }

            private struct SimpleDirectProcessor : IDsDirectProcessor<byte, int>
            {
                private readonly SimpleProcessorBlock _owner;

                public SimpleDirectProcessor(SimpleProcessorBlock owner)
                {
                    _owner = owner;
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Task Process(
                    ReadOnlySpan<byte> p1,
                    ReadOnlySpan<byte> p2,
                    Span<int> t1,
                    Span<int> t2,
                    out int processedInput,
                    out int processed)
                {
                    // Do nothing.
                    processedInput = processed = p1.Length + p2.Length;
                    return TaskEx.CompletedTaskValue;
                }
            }
        }

        private class SimpleSource
        {
            private readonly IDsProcessorBlock<byte> _processor;

            private byte[] _data = { 12, 145, 33, 22 };

            public SimpleSource(IDsProcessorBlock<byte> processor)
            {
                _processor = processor;
            }

            public void DoOperations(int count)
            {
                for (int i = 0; i < count; i++)
                {
                    _processor.Process(
                        new ReadOnlySpan<byte>(_data, 2, 2),
                        new ReadOnlySpan<byte>(_data, -0, 1),
                        out var _);
                }
            }
        }

        private class SimpleTarget : IDsTarget<int>
        {
            /// <inheritdoc/>
            public Task Process<TProcessor>(TProcessor processor)
                where TProcessor : IDsProcessor<int>
            {
                throw new NotImplementedException();
            }

            public Task ProcessDirect<TDirectProcessor, TInput>(
                TDirectProcessor directProcessor,
                ReadOnlySpan<TInput> s1,
                ReadOnlySpan<TInput> s2,
                out int inputProcessed)
                where TDirectProcessor : IDsDirectProcessor<TInput, int>
            {
                return directProcessor.Process(s1, s2, new Span<int>(), new Span<int>(), out inputProcessed, out var _);
            }
        }
    }
}
#endif