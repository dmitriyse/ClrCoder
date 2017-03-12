// <copyright file="IxProviderNode.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    /// <summary>
    /// Provider node <c>base</c> implementation.
    /// </summary>
    public abstract class IxProviderNode
    {
        private readonly List<IxProviderNode> _nodes;

        private readonly Dictionary<IxIdentifier, IxProviderNode> _nodesById;

        public IxProviderNode(
            IxHost host,
            [CanBeNull] IxProviderNode parentNode,
            IxProviderNodeConfig config,
            [CanBeNull] IxInstanceFactory instanceFactory,
            IxVisibilityFilter exportFilter,
            IxVisibilityFilter exportToParentFilter,
            IxVisibilityFilter importFilter,
            IxScopeBinderDelegate scopeBinder,
            IxDisposeHandlerDelegate disposeHandler)
        {
            if (host == null)
            {
                throw new ArgumentNullException(nameof(host));
            }

            if (exportFilter == null)
            {
                throw new ArgumentNullException(nameof(exportFilter));
            }

            if (exportToParentFilter == null)
            {
                throw new ArgumentNullException(nameof(exportToParentFilter));
            }

            if (importFilter == null)
            {
                throw new ArgumentNullException(nameof(importFilter));
            }

            if (scopeBinder == null)
            {
                throw new ArgumentNullException(nameof(scopeBinder));
            }

            if (disposeHandler == null)
            {
                throw new ArgumentNullException(nameof(disposeHandler));
            }

            _nodes = new List<IxProviderNode>();
            _nodesById = new Dictionary<IxIdentifier, IxProviderNode>();

            Host = host;
            ParentNode = parentNode;

            if (config.Identifier == null)
            {
                // ReSharper disable once NotResolvedInText
                throw new ArgumentNullException("config.Identifier");
            }

            Identifier = (IxIdentifier)config.Identifier;
            InstanceFactory = instanceFactory;
            ExportFilter = exportFilter;
            ExportToParentFilter = exportToParentFilter;
            ImportFilter = importFilter;
            ScopeBinder = scopeBinder;
            DisposeHandler = disposeHandler;

            ParentNode?.RegisterChild(this);
        }

        [CanBeNull]
        public IxInstanceFactory InstanceFactory { get; }

        public IxVisibilityFilter ExportFilter { get; }

        public IxVisibilityFilter ExportToParentFilter { get; }

        public IxVisibilityFilter ImportFilter { get; }

        public IxScopeBinderDelegate ScopeBinder { get; }

        public IxDisposeHandlerDelegate DisposeHandler { get; }

        public IxIdentifier Identifier { get; }

        public IxProviderNode ParentNode { get; }

        /// <summary>
        /// Owner <see cref="IxHost"/> <c>object</c>.
        /// </summary>
        public IxHost Host { get; }

        public IReadOnlyList<IxProviderNode> Nodes => _nodes;

        public IReadOnlyDictionary<IxIdentifier, IxProviderNode> NodesById => _nodesById;

        public Dictionary<IxIdentifier, IxResolvePath> VisibleNodes { get; } =
            new Dictionary<IxIdentifier, IxResolvePath>();

        public abstract Task<IIxInstanceLock> GetInstance(IIxInstance parentInstance, IxHost.IxResolveContext context);

        public void RegisterChild(IxProviderNode child)
        {
            _nodes.Add(child);
            _nodesById.Add(child.Identifier, child);
            VisibleNodes[child.Identifier] = new IxResolvePath(this, new[] { child });
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Identifier.ToString();
        }
    }
}