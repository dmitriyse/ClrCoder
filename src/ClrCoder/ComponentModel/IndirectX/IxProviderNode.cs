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
    /// Base encapsulation primitive.
    /// </summary>
    public abstract class IxProviderNode
    {
        private readonly List<IxProviderNode> _nodes;

        private readonly Dictionary<IxIdentifier, IxProviderNode> _nodesById;

        public IxProviderNode(
            IxHost host,
            [CanBeNull] IxProviderNode parentNode,
            IxScopeBaseConfig config,
            [CanBeNull] IxHost.RawInstanceFactory rawInstanceFactory)
        {
            if (host == null)
            {
                throw new ArgumentNullException(nameof(host));
            }

            _nodes = new List<IxProviderNode>();
            _nodesById = new Dictionary<IxIdentifier, IxProviderNode>();

            Host = host;
            ParentNode = parentNode;
            Identifier = config.Identifier;
            RawInstanceFactory = rawInstanceFactory;

            ParentNode?.RegisterChild(this);
        }

        [CanBeNull]
        public IxHost.RawInstanceFactory RawInstanceFactory { get; }

        public IxIdentifier Identifier { get; }

        public IxProviderNode ParentNode { get; }

        /// <summary>
        /// Owner <see cref="IxHost"/> <c>object</c>.
        /// </summary>
        public IxHost Host { get; }

        public IReadOnlyList<IxProviderNode> Nodes => _nodes;

        public IReadOnlyDictionary<IxIdentifier, IxProviderNode> NodesById => _nodesById;

        public abstract Task<IIxInstance> GetInstance(IIxInstance parentInstance, IxHost.IxResolveContext context);

        public void RegisterChild(IxProviderNode child)
        {
            _nodes.Add(child);
            _nodesById.Add(child.Identifier, child);
        }
    }
}