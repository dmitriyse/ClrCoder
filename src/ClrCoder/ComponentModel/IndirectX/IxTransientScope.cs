// <copyright file="IxTransientScope.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Threading;

    /// <summary>
    /// Transient dependencies should be bound to this transient scope which in turn bound to root scope.
    /// Transient scopes have unlimited multiplicity.
    /// </summary>
    [Obsolete("Probably this is wrong direction. And will be removed soon.")]
    public class IxTransientScope : IxProviderNode
    {
        public IxTransientScope(
            IxHost host,
            IxProviderNode parentNode,
            IxProviderNodeConfig config,
            IxInstanceFactory instanceFactory,
            IxVisibilityFilter exportFilter,
            IxVisibilityFilter exportToParentFilter,
            IxVisibilityFilter importFilter)
            : base(
                host,
                parentNode,
                config,
                instanceFactory,
                exportFilter,
                exportToParentFilter,
                importFilter,
                (a, b, c, d, e) => { throw new NotImplementedException(); },
                obj => TaskEx.CompletedTask)
        {
        }

        public override ValueTask<IIxInstanceLock> GetInstance(
            IIxInstance parentInstance,
            IxIdentifier identifier,
            IxHost.IxResolveContext context,
            [CanBeNull] IxResolveFrame frame)
        {
            throw new NotImplementedException();
        }
    }
}