// <copyright file="VxArgsTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests.Validation
{
    using System;

    using ClrCoder.Validation;

    using FluentAssertions;

    using NUnit.Framework;

    /// <summary>
    /// Tests for the <see cref="VxArgs"/>.
    /// </summary>
    [TestFixture]
    public class VxArgsTests
    {
        /// <summary>
        /// Checks <see cref="VxArgs.ValidAbsoluteUri"/>.
        /// </summary>
        [Test]
        public void ValidAbsoluteUriTest()
        {
            // ReSharper disable once NotResolvedInText
            Action a = () => { VxArgs.ValidAbsoluteUri("http://valid.uri", "test"); };
            a();

            // ReSharper disable once NotResolvedInText
            a = () => { VxArgs.ValidAbsoluteUri("/../valid.uri", "test"); };
            a.ShouldThrow<ArgumentException>().WithInnerException<UriFormatException>();
        }
    }
}