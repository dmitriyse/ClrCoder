// <copyright file="FlurlBehaviorTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests.Net.Http
{
    using Flurl;

    using Newtonsoft.Json;

    using NUnit.Framework;

    /// <summary>
    /// Tests flurl implementation.
    /// </summary>
    [TestFixture]
    public class FlurlBehaviorTests
    {
        /// <summary>
        /// Tests json serialization behavior.
        /// </summary>
        [Test]
        [Ignore("For manual testing")]
        public void JsonSerializeTest()
        {
            string result = JsonConvert.SerializeObject(
                new Url("http://test.com"),
                JsonDefaults.JsonConfigSerializerSettings);
            TestContext.WriteLine(result);
        }
    }
}