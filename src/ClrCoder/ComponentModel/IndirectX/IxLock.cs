// <copyright file="IxLock.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;

    public class IxLock<T> : IDisposable
    {
        private readonly IIxInstanceLock _instanceLock;

        public IxLock(IIxInstanceLock instanceLock)
        {
            if (instanceLock == null)
            {
                throw new ArgumentNullException(nameof(instanceLock));
            }

            _instanceLock = instanceLock;
            Target = (T)_instanceLock.Target;
        }

        public T Target { get; }

        public void Dispose()
        {
            _instanceLock.Dispose();
        }
    }
}