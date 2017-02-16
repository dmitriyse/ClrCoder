// <copyright file="HostUtilsTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests.AspNetCore.Hosting
{
#if NET46 || NETCOREAPP1_1
    using System.Threading.Tasks;

    using ClrCoder.AspNetCore.Hosting;

    using FluentAssertions;

    using Flurl;
    using Flurl.Http;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="HostUtils"/>.
    /// </summary>
    [TestFixture]
    public class HostUtilsTests
    {
        /// <summary>
        /// Test for the <see cref="HostUtils.HostController{TController}"/> method.
        /// </summary>
        /// <returns>Async execution TPL task.</returns>
        [Test]
        public async Task HostControllerTest()
        {
            var baseUrl = "http://localhost:5082";

            using (IWebHost host = HostUtils.HostController<RightController>(baseUrl))
            {
                (await new Url(baseUrl).AppendPathSegment("/tst").GetAsync().ReceiveString())
                    .Should().Be("RightController");
            }
        }

        [Route("/tst")]
        private class WrongController
        {
            [HttpGet]
            public async Task<string> Get()
            {
                return "WrongController";
            }
        }

        [Route("/tst")]
        private class RightController
        {
            [HttpGet]
            public async Task<string> Get()
            {
                return "RightController";
            }
        }
    }


#endif
}