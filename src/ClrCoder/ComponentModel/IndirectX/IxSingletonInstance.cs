// <copyright file="IxSingletonInstance.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;
    using System.Threading.Tasks;

    public class IxSingletonInstance : IxInstance
    {
        public IxSingletonInstance(IxHost host, IxProviderNode providerNode, IIxInstance parentInstance, object @object)
            : base(host, providerNode, parentInstance, @object)
        {
            if (parentInstance == null)
            {
                throw new ArgumentNullException(nameof(parentInstance));
            }
        }

        public override async Task AsyncDispose()
        {
            // TODO: Instance should be disposed with parent scope disposing.
            // Do nothing.
        }
    }
}