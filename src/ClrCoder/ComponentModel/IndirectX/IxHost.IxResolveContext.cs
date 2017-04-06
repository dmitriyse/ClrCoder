// <copyright file="IxHost.IxResolveContext.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using JetBrains.Annotations;

    /// <content>Resolve context implementation.</content>
    public partial class IxHost
    {
        /// <summary>
        /// Resolve context.
        /// </summary>
        public class IxResolveContext
        {
            private readonly IIxInstance _originInstance;

            private readonly IxResolveContext _rootContext;

            [CanBeNull]
            private Dictionary<IxProviderNode, object> _providersData;

            public IxResolveContext(
                IIxInstance originInstance,
                [CanBeNull] IxResolveContext parentContext,
                IReadOnlyDictionary<IxIdentifier, object> arguments)
            {
                Arguments = arguments;
                _originInstance = originInstance;
                ParentContext = parentContext;
                _rootContext = parentContext?._rootContext ?? this;
            }

            [CanBeNull]
            public IxResolveContext ParentContext { get; }

            [NotNull]
            public IReadOnlyDictionary<IxIdentifier, object> Arguments { get; }

            private Dictionary<IxProviderNode, object> ProvidersData
            {
                get
                {
                    if (_providersData == null)
                    {
                        Interlocked.CompareExchange(ref _providersData, new Dictionary<IxProviderNode, object>(), null);
                    }

                    return _providersData;
                }
            }

            /// <inheritdoc/>
            [CanBeNull]
            public object GetData(IxProviderNode providerNode)
            {
                if (providerNode == null)
                {
                    throw new ArgumentNullException(nameof(providerNode));
                }

                if (!Monitor.IsEntered(_originInstance.ProviderNode.Host.InstanceTreeSyncRoot))
                {
                    Critical.Assert(false, "Data manipulations should be performed under lock.");
                }

                object result;

                _rootContext.ProvidersData.TryGetValue(providerNode, out result);

                return result;
            }

            /// <inheritdoc/>
            public void SetData(IxProviderNode providerNode, [CanBeNull] object data)
            {
                if (providerNode == null)
                {
                    throw new ArgumentNullException(nameof(providerNode));
                }

                if (!Monitor.IsEntered(_originInstance.ProviderNode.Host.InstanceTreeSyncRoot))
                {
                    Critical.Assert(false, "Data manipulations should be performed under tree lock.");
                }

                if (data == null)
                {
                    _rootContext.ProvidersData.Remove(providerNode);
                }
                else
                {
                    _rootContext.ProvidersData[providerNode] = data;
                }
            }
        }
    }
}