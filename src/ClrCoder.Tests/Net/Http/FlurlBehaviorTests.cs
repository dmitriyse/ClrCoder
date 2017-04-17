// <copyright file="FlurlBehaviorTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests.Net.Http
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using ClrCoder.Json;
    using ClrCoder.Logging;
    using ClrCoder.Net.Http;
    using ClrCoder.Threading;
    using ClrCoder.AspNetCore.Hosting;

    using FluentAssertions;

    using Microsoft.AspNetCore.Mvc;

    using Flurl;
    using Flurl.Http;

    using Newtonsoft.Json;

    using NUnit.Framework;

    using Testing;

    /// <summary>
    /// Tests flurl implementation.
    /// </summary>
    [TestFixture]
    public class FlurlBehaviorTests
    {
        /// <summary>
        /// Test for connecting to unknown DNS name.
        /// </summary>
        /// <param name="host"> The <c>host</c>. </param>
        /// <param name="dnsOrIp"> The dns or IP. </param>
        /// <param name="pathSegment"> The path Segment. </param>
        /// <returns>
        /// Async execution TPL task.
        /// </returns>
        [Test]
        [TestCase("http://localhost:5001", "http://localhost:5001", "Flurl")]
        [TestCase("http://localhost:5001", "http://localhost:5001", "nFlurl")]
        [TestCase("http://localhost:5001", "http://nobody-knows-me-99923.com", "Flurl")]
        [TestCase("http://localhost:5001", "http://10.110.110.110/", "nFlurl")]
        [TestCase("http://localhost:5001", "http://46.20.65.243:22222/", "")]
        public async Task FlurlHttpExtensionsTest(string host, string dnsOrIp, string pathSegment)
        {
            try
            {
                using (HostUtils.HostController<TestFlurlController>(host))
                {
                    string response = await new Url(dnsOrIp).AppendPathSegment(pathSegment).GetAsync().ReceiveString();
                    if (pathSegment == "Flurl" && host == "http://localhost:5001")
                    {
                        response.Should().BeEquivalentTo("Hello");
                    }
                }
            }
            catch (FlurlHttpTimeoutException)
            {
                TestContext.WriteLine("FlurlHttpTimeoutException");
            }
            catch (FlurlHttpException ex)
            {
                if (dnsOrIp == "http://nobody-knows-me-99923.com")
                {
                    ex.IsDnsResolveError().Should().Be(true);
                }
                else
                {
                    ex.IsDnsResolveError().Should().Be(false);
                }

                if (dnsOrIp == "http://10.110.110.110/" || dnsOrIp == "http://46.20.65.243:22222/")
                {
                    ex.IsIpNotAvailable().Should().Be(true);
                }
                else
                {
                    ex.IsIpNotAvailable().Should().Be(false);
                }
            }
        }

        /// <summary>
        /// Tests json serialization behavior.
        /// </summary>
        [Test]
        [Ignore("For manual testing")]
        public void JsonSerializeTest()
        {
            string result = JsonConvert.SerializeObject(
                new Url("http://test.com"),
                JsonDefaults.JsonConfigSerializerSource.Settings);
            TestContext.WriteLine(result);
        }

        [Route("/Flurl")]
        public class TestFlurlController
        {
            [HttpGet]
            public string Get()
            {
                return "Hello";
            }

            [HttpPost]
            public string Post()
            {
                return "Bye";
            }
        }
    }
}