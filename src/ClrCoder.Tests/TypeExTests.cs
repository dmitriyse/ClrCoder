// <copyright file="TypeExTests.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Tests
{
    using System;

    using FluentAssertions;

    using NUnit.Framework;

    using Reflection;

    /// <summary>
    /// Tests for <see cref="TypeEx"/> and <see cref="TypeEx{T}"/> classes.
    /// </summary>
    [TestFixture]
    public class TypeExTests
    {
        /// <summary>
        /// Test for the <see cref="TypeEx{T}.CreateConstructorDelegate"/> method.
        /// </summary>
        [Test]
        public void CreateConstructorDelegateClass()
        {
            Func<object> objConstructor = TypeEx<object>.CreateConstructorDelegate();
            object obj = objConstructor.Invoke();

            obj.Should().NotBeNull();
        }
    }
}