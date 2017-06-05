// <copyright file="AsyncResponseHandlerTest.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests.AspNetCore.AsyncResponse
{
    using System;
    using System.Threading.Tasks;

    using ClrCoder.AspNetCore.AsyncResponse;

    using Flurl;
    using Flurl.Http;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.DependencyInjection;

    using NUnit.Framework;

    using Testing;

    [TestFixture]
    public class AsyncResponseHandlerTest
    {
        private string _baseUrl = "http://localhost:5321";

        [Test]
        public async Task SimpleTest()
        {
            var asyncResponseHandler = new AsyncResponseHandler<string>(
                httpContext => httpContext.Request.Headers["r-key"],
                new NUnitJsonLogger());

            IWebHostBuilder builder = new WebHostBuilder()
                .UseKestrel()
                .ConfigureServices(services => services.AddMvc())
                .UseUrls(_baseUrl.Replace("localhost", "*"))
                .Configure(
                    app =>
                        {
                            app.UseMvc(
                                routes => { routes.MapPost("test/url", asyncResponseHandler.RequestHandler); });
                        });

            using (IWebHost host = builder.Build())
            {
                host.Start();
                Url url = new Url(_baseUrl).AppendPathSegment("test/url");
                Task<string> sendTask = url.WithHeader("r-key", "t1").PostStringAsync("test").ReceiveString();

                Task responseTask = asyncResponseHandler.WaitAndHandleAsyncResponse(
                    "t1",
                    async (key, httpContext) =>
                        {
                            httpContext.Response.StatusCode = 200;
                            httpContext.Response.ContentType = "text/plain";
                            await httpContext.Response.WriteAsync("Hello!");
                        },
                    TimeSpan.FromSeconds(30));
                string result = await sendTask;
                TestContext.WriteLine(result);
                await responseTask;
            }
        }
    }
}