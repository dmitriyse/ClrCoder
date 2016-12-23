// <copyright file="TracingDelegatingHandlerTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests.Net.Http
{
    using System.Net.Http;
    using System.Threading.Tasks;

    using ClrCoder.Net.Http;

    using Newtonsoft.Json;

    using NUnit.Framework;
    using NUnit.Framework.Internal;

    /// <summary>
    /// Tests for the class <see cref="TracingDelegatingHandler"/>.
    /// </summary>
    [TestFixture]
    public class TracingDelegatingHandlerTests
    {
        /// <summary>
        /// Simple test.
        /// </summary>
        /// <returns>Async execution task.</returns>
        [Test]
        public async Task SimpleTest()
        {
            var tracingHandler = new TracingDelegatingHandler(new HttpClientHandler());
            tracingHandler.MessageProcessed +=
                (sender, args) =>
                    {
                        TestExecutionContext.CurrentContext.OutWriter.WriteLine(
                            JsonConvert.SerializeObject(args, Formatting.Indented));
                    };
            var httpClient = new HttpClient(tracingHandler);

            try
            {
                await httpClient.PostAsync("https://google.com", new StringContent("Hello world!"));
            }
            catch
            {
                // do nothing.
            }
        }
    }
}