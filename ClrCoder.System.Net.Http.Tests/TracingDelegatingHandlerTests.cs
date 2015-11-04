using System.Diagnostics;
using System.Net.Http;

using Newtonsoft.Json;

using NUnit.Framework;

namespace ClrCoder.System.Net.Http.Tests
{
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