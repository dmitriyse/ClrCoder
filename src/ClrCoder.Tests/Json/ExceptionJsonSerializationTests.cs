// <copyright file="ExceptionJsonSerializationTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests.Json
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    using NUnit.Framework;

    using Runtime.Serialization;

    /// <summary>
    /// Tests related to the exceptions serialization.
    /// </summary>
    [TestFixture]
    public class ExceptionJsonSerializationTests
    {
        /// <summary>
        /// Aggregate exception serialization test.
        /// </summary>
        [Test]
        public void AggregateExceptionSerializeTest()
        {
            Exception aggregateError = null;
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            Func<Task> asyncFunc = async () =>
                {
                    File.ReadAllText("some invalid path.txt");
                    Assert.Fail();
                };
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

            try
            {
                asyncFunc().Wait();
            }
            catch (Exception ex)
            {
                aggregateError = ex;
            }

            var exceptionDumper = new JsonExceptionDumper();

            Debug.Assert(aggregateError != null, "aggregateError != null");
            var dto = exceptionDumper.Dump<ExceptionDto>(aggregateError);
            TestContext.WriteLine(JsonConvert.SerializeObject(dto, JsonConfig.SerializerSettings));
        }

        /// <summary>
        /// Simple serialize test.
        /// </summary>
        [Test]
        public void SimpleSerializeTest()
        {
            Exception simpleError = null;
            try
            {
                File.ReadAllText("some invalid path.txt");
                Assert.Fail();
            }
            catch (Exception ex)
            {
                simpleError = ex;
            }

            var exceptionDumper = new JsonExceptionDumper();
            var dto = exceptionDumper.Dump<ExceptionDto>(simpleError);
            TestContext.WriteLine(JsonConvert.SerializeObject(dto, JsonConfig.SerializerSettings));
        }
    }
}