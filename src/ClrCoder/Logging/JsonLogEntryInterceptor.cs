// <copyright file="JsonLogEntryInterceptor.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Logging
{
    /// <summary>
    /// <see cref="IJsonLogger"/> log entry interceptor delegate. Helps to abstract log entry modification.
    /// </summary>
    /// <param name="builder">The original builder.</param>
    /// <returns>The intercepted builder.</returns>
    public delegate ILogEntryBuilder JsonLogEntryInterceptor(ILogEntryBuilder builder);
}