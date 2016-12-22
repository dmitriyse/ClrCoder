// <copyright file="MicroBenchmarks.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
namespace ClrCoder.Tests
{
    using System;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;

    using NUnit.Framework;

    /// <summary>
    /// BCL micro benchmarks.
    /// </summary>
    [TestFixture]
    public class MicroBenchmarks
    {
        /// <summary>
        /// Core i7-5920k@4.5Ghz speed = ~36M op/s.
        /// </summary>
        [Test]
        public void AnonymousFuncCall()
        {
            var str = "tst";
            var testSize = 10000000;

            var stopwatch = Stopwatch.StartNew();
            for (var i = 0; i < testSize; i++)
            {
                var tstStr = GetStringByAnonymousFunc(str);
                if (tstStr.Length != 3)
                {
                    break;
                }
            }

            stopwatch.Stop();
            TestContext.WriteLine((testSize * 1000.0) / stopwatch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Core i7-5920k@4.5Ghz speed = ~2.4M op/s.
        /// </summary>
        [Test]
        public void ExpressionGenNameTest()
        {
            var str = "tst";
            var testSize = 10000000;
            TestContext.Out.WriteLine(GetExpressionName(() => str));

            var stopwatch = Stopwatch.StartNew();
            for (var i = 0; i < testSize; i++)
            {
                var name = GetExpressionName(() => str);
                if (name.Length != 3)
                {
                    break;
                }
            }

            stopwatch.Stop();
            TestContext.WriteLine((testSize * 1000.0) / stopwatch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Core i7-5920k@4.5Ghz speed = ~5.1K op/s. 
        /// </summary>
        [Test]
        public void LambdaCompileBenchmark()
        {
            var str = "tst";
            var testSize = 10000;

            var stopwatch = Stopwatch.StartNew();
            for (var i = 0; i < testSize; i++)
            {
                Expression<Func<string>> labmda = () => str;
                var tstStr = labmda.Compile()();
                if (tstStr.Length != 3)
                {
                    break;
                }
            }

            stopwatch.Stop();
            TestContext.WriteLine((testSize * 1000.0) / stopwatch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Core i7-5920k@4.5Ghz speed = ~47M op/s.
        /// </summary>
        [Test]
        public void ObjectCreationBenchmark()
        {
            // ReSharper disable once UnusedVariable
            var obj = new object();

            var testSize = 10000000;
            var stopwatch = Stopwatch.StartNew();
            for (var i = 0; i < testSize; i++)
            {
                if (new object().GetHashCode() == 42)
                {
                    i++;
                }
            }

            stopwatch.Stop();
            TestContext.WriteLine((testSize * 1000.0) / stopwatch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Core i7-5920k@4.5Ghz speed = ~2.4M op/s.
        /// </summary>
        /// <typeparam name="T">Type of result.</typeparam>
        /// <param name="getNameExpression">Expression with encoded parameter name.</param>
        /// <returns>Extracted parameter name.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string GetExpressionName<T>(Expression<Func<T>> getNameExpression)
        {
            var expression = (MemberExpression)getNameExpression.Body;
            return expression.Member.Name;
        }

        private string GetStringByAnonymousFunc(string str)
        {
            Func<string> f = () => str;
            return f();
        }
    }
}