// <copyright file="IxExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// IndirectX helpers and fluent API syntax extension.
    /// </summary>
    public static class IxExtensions
    {
        /// <summary>
        /// Adds config node to nodes list builder.
        /// </summary>
        /// <param name="nodesBuilder">Nodes list builder.</param>
        /// <param name="nodeConfig">Config to add.</param>
        /// <returns>Fluent syntax continuation.</returns>
        public static IIxBuilder<ICollection<IIxProviderNodeConfig>> Add(
            this IIxBuilder<ICollection<IIxProviderNodeConfig>> nodesBuilder,
            IIxProviderNodeConfig nodeConfig)
        {
            if (nodesBuilder == null)
            {
                throw new ArgumentNullException(nameof(nodesBuilder));
            }

            if (nodeConfig == null)
            {
                throw new ArgumentNullException(nameof(nodeConfig));
            }

            nodesBuilder.Config.Add(nodeConfig);
            return nodesBuilder;
        }

        /// <summary>
        /// Adds standard provider node.
        /// </summary>
        /// <typeparam name="TContract">Registration target type.</typeparam>
        /// <param name="nodesBuilder">Nodes builder.</param>
        /// <param name="name">Registration <c>name</c>.</param>
        /// <param name="scopeBinding">Scope binding strategy config (registration, transient, etc.).</param>
        /// <param name="importFilter">Import filter. Controls which registrations of parent node are visible for current node.</param>
        /// <param name="exportToParentFilter">
        /// Export to parent filter. Controls which registrations of <c>this</c> node will be
        /// visible in parent node.
        /// </param>
        /// <param name="exportFilter">Export to children filter. Controls which registrations of <c>this</c> node.</param>
        /// <param name="factory">Instance builder config. (Class constructor, existing instance, etc.).</param>
        /// <param name="multiplicity">Multiplicity config. (Singleton, pool, <c>factory</c> etc.).</param>
        /// <param name="disposeHandler">Overrides dispose operation.</param>
        /// <param name="nodes">Action that build nested <c>nodes</c>.</param>
        /// <returns>Fluent syntax continuation.</returns>
        public static IIxBuilder<ICollection<IIxProviderNodeConfig>> Add<TContract>(
            this IIxBuilder<ICollection<IIxProviderNodeConfig>> nodesBuilder,
            string name = null,
            IIxScopeBindingConfig scopeBinding = null,
            IIxVisibilityFilterConfig importFilter = null,
            IIxVisibilityFilterConfig exportToParentFilter = null,
            IIxVisibilityFilterConfig exportFilter = null,
            IIxInstanceBuilderConfig factory = null,
            IIxMultiplicityConfig multiplicity = null,
            IxDisposeHandlerDelegate disposeHandler = null,
            Action<IIxBuilder<ICollection<IIxProviderNodeConfig>>> nodes = null)
        {
            var depNode = new IxStdProviderConfig
                              {
                                  Factory = factory,
                                  Identifier = new IxIdentifier(typeof(TContract), name),
                                  ScopeBinding = scopeBinding,
                                  Multiplicity = multiplicity,
                                  ImportFilter = importFilter,
                                  ExportFilter = exportFilter,
                                  ExportToParentFilter = exportToParentFilter,
                                  DisposeHandler = disposeHandler
                              };

            nodesBuilder.Config.Add(depNode);

            nodes?.Invoke(
                new IxBuilder<ICollection<IIxProviderNodeConfig>>
                    {
                        Config = depNode.Nodes
                    });

            return nodesBuilder;
        }

        /// <summary>
        /// Adds scope to a node.
        /// </summary>
        /// <param name="nodesBuilder">Nodes builder of a parent node.</param>
        /// <param name="name">Scope should be named.</param>
        /// <param name="importFilter">Import filter. Controls which registrations of parent node are visible for current node.</param>
        /// <param name="exportToParentFilter">
        /// Export to parent filter. Controls which registrations of <c>this</c> node will be
        /// visible in parent node.
        /// </param>
        /// <param name="exportFilter">Export to children filter. Controls which registrations of <c>this</c> node.</param>
        /// <param name="nodes">Action that builds children <c>nodes</c>.</param>
        /// <returns>Fluent syntax continuation.</returns>
        public static IIxBuilder<ICollection<IIxProviderNodeConfig>> AddScope(
            this IIxBuilder<ICollection<IIxProviderNodeConfig>> nodesBuilder,
            string name = null,
            IIxVisibilityFilterConfig importFilter = null,
            IIxVisibilityFilterConfig exportToParentFilter = null,
            IIxVisibilityFilterConfig exportFilter = null,
            Action<IIxBuilder<ICollection<IIxProviderNodeConfig>>> nodes = null)
        {
            if (nodesBuilder == null)
            {
                throw new ArgumentNullException(nameof(nodesBuilder));
            }

            var scopeConfig = new IxScopeConfig
                                  {
                                      Identifier = new IxIdentifier(typeof(IxScope), name),
                                      ExportToParentFilter = exportToParentFilter,
                                      ExportFilter = exportFilter,
                                      ImportFilter = importFilter
                                  };

            nodesBuilder.Config.Add(scopeConfig);
            nodes?.Invoke(
                new IxBuilder<ICollection<IIxProviderNodeConfig>>
                    {
                        Config = scopeConfig.Nodes
                    });

            return nodesBuilder;
        }

        /// <summary>
        /// Gets locks on the required <c>object</c> from the specified <c>resolver</c>.
        /// </summary>
        /// <typeparam name="T">Type of target <c>object</c>.</typeparam>
        /// <param name="resolver">Resolver that should be used.</param>
        /// <param name="name">Name of registration.</param>
        /// <returns>Temp <c>lock</c> on the <c>object</c>.</returns>
        public static async Task<IxLock<T>> Get<T>(this IIxResolver resolver, string name = null)
        {
            if (resolver == null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            return new IxLock<T>(await resolver.Resolve(new IxIdentifier(typeof(T), name)));
        }
    }
}