// <copyright file="IxDelegateFactoryConfig.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;

    public class IxDelegateFactoryConfig : IIxInstanceBuilderConfig
    {
        public IxDelegateFactoryConfig(Delegate factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            Factory = factory;
        }

        public Delegate Factory { get; }
    }
}