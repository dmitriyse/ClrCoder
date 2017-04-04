// <copyright file="RootTestingEnvironment.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests
{
    using NUnit.Framework;

    using Testing;

    /// <summary>
    /// Prepares environment for all tests.
    /// </summary>
    [TestFixture]
    public class RootTestingEnvironment
    {
        /// <summary>
        /// Logger for tests.
        /// </summary>
        public static NUnitJsonLogger Logger { get; } = new NUnitJsonLogger();

        /// <summary>
        /// Performs initialization for all tests.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
        }

        /// <summary>
        /// Performs
        /// </summary>
        [OneTimeTearDown]
        public void Shutdown()
        {
        }
    }
}