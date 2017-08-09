﻿// <copyright file="IIxHost.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System.Threading;

    using Threading;

    public interface IIxHost : IAsyncDisposable
    {
        IIxResolver Resolver { get; }
    }
}