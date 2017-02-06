// <copyright file="Class.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    public class IxRawInstanceFactory
    {
        public IxRawInstanceFactory(IxRawInstanceFactoryDelegate factory, [CanBeNull] Type instanceBaseType)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            Factory = factory;
            InstanceBaseType = instanceBaseType;
        }

        public IxRawInstanceFactoryDelegate Factory { get; }

        [CanBeNull]
        public Type InstanceBaseType { get; }
    }

    /// <summary>
    /// Raw instance factory <c>delegate</c>. No any registrations just obtain instance according to config.
    /// </summary>
    /// <param name="parentInstance">Parent instance.</param>
    /// <param name="context"><c>Resolve</c> <c>context</c>.</param>
    /// <returns>Create instance.</returns>
    public delegate Task<object> IxRawInstanceFactoryDelegate(
        IIxInstance parentInstance,
        IxHost.IxResolveContext context);
}