// <copyright file="FlurlExtensionsTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests.Net.Http
{
    // TODO: Add NETCOREAPP1_1, msbuild have bug and currently can't build the project.
#if NETCOREAPP2_0 || NET461
    using System.Threading.Tasks;

    using ClrCoder.AspNetCore.Hosting;
    using ClrCoder.Logging;
    using ClrCoder.Net.Http;
    using ClrCoder.Threading;

    using FluentAssertions;

    using Flurl;
    using Flurl.Http;

    using Microsoft.AspNetCore.Mvc;

    using Newtonsoft.Json;

    using NUnit.Framework;

    using Runtime.Serialization;

    using Testing;

    /// <summary>
    /// Tests for the <see cref="FlurlHttpExtensions"/>.
    /// </summary>
    [TestFixture]
    public class FlurlExtensionsTests
    {
        /// <summary>
        /// Test for connecting to unknown DNS name. // TODO: Rewrite test.
        /// </summary>
        /// <param name="host"> The <c>host</c>. </param>
        /// <param name="dnsOrIp"> The dns or IP. </param>
        /// <param name="pathSegment"> The path Segment. </param>
        /// <returns>
        /// Async execution TPL task.
        /// </returns>
        [Test]
        [TestCase("http://localhost:5062", "http://localhost:5062", "Flurl")]
        [TestCase("http://localhost:5062", "http://localhost:5062", "nFlurl")]
        [TestCase("http://localhost:5062", "http://nobody-knows-me-99923.com", "Flurl")]
        [TestCase("http://localhost:5062", "http://10.110.110.110/", "nFlurl")]
        [TestCase("http://localhost:5062", "http://46.20.65.243:22222/", "")]
        public async Task FlurlHttpExtensionsTest(string host, string dnsOrIp, string pathSegment)
        {
            try
            {
                using (HostUtils.HostController<TestFlurlController>(host))
                {
                    string response = await new Url(dnsOrIp).AppendPathSegment(pathSegment).GetAsync().ReceiveString();
                    if ((pathSegment == "Flurl") && (host == "http://localhost:5062"))
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

                if ((dnsOrIp == "http://10.110.110.110/") || (dnsOrIp == "http://46.20.65.243:22222/"))
                {
                    ex.IsIpNotAvailable().Should().Be(true);
                }
                else
                {
                    ex.IsIpNotAvailable().Should().Be(false);
                }
            }
        }

        [Test]
        public async Task SendJsonAsyncWithDumpTest()
        {
            var data = new MethodInfoDto
                           {
                               Name = "name",
                               TypeFullName = "fullname"
                           };

            IJsonLogger logger = new NUnitJsonLogger(SyncHandler.Instance);

            using (HostUtils.HostController<TestFlurlController>("http://localhost:5062"))
            {
                string response = await new Url("http://localhost:5062")
                                      .AppendPathSegment("Flurl")
                                      .WithDump(logger)
                                      .PostJsonAsync(data)
                                      .ReceiveString();

                var responseData = JsonConvert.DeserializeObject<MethodInfoDto>(response);
                responseData.Should().NotBeNull();
                responseData.Name.Should().Be(data.Name);
                responseData.TypeFullName.Should().Be(data.TypeFullName);
            }
        }

        /// <summary>
        /// Controller for testing flurl extensions.
        /// </summary>
        [Route("/Flurl")]
        private class TestFlurlController
        {
            [HttpGet]
            public string Get()
            {
                return "Hello";
            }

            [HttpPost]
            public string Post([FromBody] MethodInfoDto data)
            {
                return JsonConvert.SerializeObject(data);
            }
        }
    }
#endif
}