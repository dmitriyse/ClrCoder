// <copyright file="TracingDelegatingHandlerTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>
namespace ClrCoder.System.Net.Http.Tests
{
    using global::System.Diagnostics;
    using global::System.Net.Http;

    using Newtonsoft.Json;

    using NUnit.Framework;

    /// <summary>
    /// Tests for the class <see cref="TracingDelegatingHandler"/>.
    /// </summary>
    [TestFixture]
    public class TracingDelegatingHandlerTests
    {
        /// <summary>
        /// Simple test.
        /// </summary>
        [Test]
        public async void SimpleTest()
        {
            var tracingHandler = new TracingDelegatingHandler(new HttpClientHandler());
            tracingHandler.MessageProcessed +=
                (sender, args) => { Trace.WriteLine(JsonConvert.SerializeObject(args, Formatting.Indented)); };
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