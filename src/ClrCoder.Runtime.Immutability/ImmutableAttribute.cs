// <copyright file="ImmutableAttribute.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace System
{
    using JetBrains.Annotations;

    /// <summary>
    /// Annotate that target should be shallow immutable.
    /// </summary>
    [PublicAPI]
    public class ImmutableAttribute : ShallowImmutableAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableAttribute"/> class.
        /// </summary>
        /// <param name="scope">The type of part/aspect of an instance that is/should be shallow immutable. null means whole object.</param>
        public ImmutableAttribute(Type scope = null): base(scope)
        {
        }
    }
}