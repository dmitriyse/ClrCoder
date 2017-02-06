// <copyright file="IxTransientScope.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Transient dependencies should be bound to this transient scope which in turn bound to root scope.
    /// Transient scopes have unlimited multiplicity.
    /// </summary>
    public class IxTransientScope : IxProviderNode
    {
        public IxTransientScope(
            IxHost host,
            IxProviderNode parentNode,
            IxScopeBaseConfig config,
            IxRawInstanceFactory rawInstanceFactory,
            IxVisibilityFilter exportFilter,
            IxVisibilityFilter exportToParentFilter,
            IxVisibilityFilter importFilter)
            : base(
                host,
                parentNode,
                config,
                rawInstanceFactory,
                exportFilter,
                exportToParentFilter,
                importFilter,
                (a, b, c, d) => { throw new NotImplementedException(); },
                obj => Task.CompletedTask)
        {
        }

        public override Task<IIxInstanceLock> GetInstance(IIxInstance parentInstance, IxHost.IxResolveContext context)
        {
            throw new NotImplementedException();
        }
    }
}