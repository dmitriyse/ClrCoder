// <copyright file="IxScopeBase.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    /// <summary>
    /// Base encapsulation primitive.
    /// </summary>
    public class IxScopeBase
    {
        private readonly List<IxScopeBase> _nodes;

        private readonly Dictionary<IxIdentifier, IxScopeBase> _nodesById;

        public IxScopeBase(
            IxHost host,
            [CanBeNull] IxScopeBase parentNode,
            IxScopeBaseConfig config)
        {
            if (host == null)
            {
                throw new ArgumentNullException(nameof(host));
            }

            _nodes = new List<IxScopeBase>();
            _nodesById = new Dictionary<IxIdentifier, IxScopeBase>();

            Host = host;
            ParentScope = parentNode;
            Identifier = config.Identifier;
            if (ParentScope != null)
            {
                ParentScope.RegisterChild(this);
            }
        }

        public IxIdentifier Identifier { get; }

        public IxScopeBase ParentScope { get; }

        /// <summary>
        /// Owner <see cref="IxHost"/> <c>object</c>.
        /// </summary>
        public IxHost Host { get; }

        public IReadOnlyList<IxScopeBase> Nodes => _nodes;

        public IReadOnlyDictionary<IxIdentifier, IxScopeBase> NodesById => _nodesById;

        public void RegisterChild(IxScopeBase child)
        {
            _nodes.Add(child);
            _nodesById.Add(child.Identifier, child);
        }
    }
}