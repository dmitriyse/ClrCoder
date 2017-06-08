// <copyright file="IxHost.IxResolveContext.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Annotations;

    using JetBrains.Annotations;

    /// <content>Resolve context implementation.</content>
    public partial class IxHost
    {
        /// <summary>
        /// Resolve context.
        /// </summary>
        [InvalidUsageIsCritical]
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
                Critical.CheckedAssert(originInstance != null, "Origin instance should be null.");
                Critical.CheckedAssert(arguments != null, "Arguments dictionary should not be null.");

                Arguments = arguments;
                _originInstance = originInstance;
                ParentContext = parentContext;
                _rootContext = parentContext?._rootContext ?? this;

                Critical.CheckedAssert(_rootContext != null, "Root context should not be null.");
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
                Critical.CheckedAssert(providerNode != null, "providerNode != null");

                if (!Monitor.IsEntered(_originInstance.ProviderNode.Host.InstanceTreeSyncRoot))
                {
                    Critical.Assert(false, "Data manipulations should be performed under lock.");
                }

                _rootContext.ProvidersData.TryGetValue(providerNode, out object result);

                return result;
            }

            /// <inheritdoc/>
            public void SetData(IxProviderNode providerNode, [CanBeNull] object data)
            {
                Critical.CheckedAssert(providerNode != null, "providerNode != null");

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