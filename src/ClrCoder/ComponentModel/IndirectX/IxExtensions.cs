// <copyright file="IxExtensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public static class IxExtensions
    {
        public static IIxBuilder<List<IxScopeBaseConfig>> Add<TContract>(
            this IIxBuilder<List<IxScopeBaseConfig>> nodesBuilder,
            string name = null,
            IIxScopeBindingConfig scopeBinding = null,
            IIxVisibilityFilterConfig importFilter = null,
            IIxVisibilityFilterConfig exportToParentFilter = null,
            IIxVisibilityFilterConfig exportFilter = null,
            IIxFactoryConfig factory = null,
            IIxMultiplicityConfig multiplicity = null,
            Action<IIxBuilder<List<IxScopeBaseConfig>>> nodes = null)
        {
            // TODO: Apply defaults.
            nodesBuilder.Config.Add(
                new IxStdProviderConfig
                    {
                        Factory = factory,
                        Identifier = new IxIdentifier(typeof(TContract), name),
                        ScopeBinding = scopeBinding,
                        Multiplicity = multiplicity ?? new IxSingletonMultiplicityConfig(),
                        ImportFilter = importFilter,
                        ExportFilter = exportFilter,
                        ExportToParentFilter = exportToParentFilter
                    });

            nodes?.Invoke(new IxBuilder<List<IxScopeBaseConfig>>());

            return nodesBuilder;
        }

        /// <summary>
        /// Adds scope to a node.
        /// </summary>
        /// <param name="nodesBuilder">Nodes builder of a parent node.</param>
        /// <param name="name">Scope should be named.</param>
        /// <param name="nodes">Action that builds children <c>nodes</c>.</param>
        /// <returns>Fluent syntax continuation.</returns>
        public static IIxBuilder<List<IxScopeBaseConfig>> AddScope(
            this IIxBuilder<List<IxScopeBaseConfig>> nodesBuilder,
            string name = null,
            IIxVisibilityFilterConfig importFilter = null,
            IIxVisibilityFilterConfig exportToParentFilter = null,
            IIxVisibilityFilterConfig exportFilter = null,
            Action<IIxBuilder<List<IxScopeBaseConfig>>> nodes = null)
        {
            if (nodesBuilder == null)
            {
                throw new ArgumentNullException(nameof(nodesBuilder));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name for scope should not be empty", nameof(name));
            }

            // TODO:Apply defaults.
            var scopeConfig = new IxScopeConfig
                                  {
                                      Identifier = new IxIdentifier(typeof(IxScope), name),
                                      ExportToParentFilter = exportToParentFilter,
                                      ExportFilter = exportFilter,
                                      ImportFilter = importFilter
                                  };

            nodesBuilder.Config.Add(scopeConfig);

            nodes?.Invoke(
                new IxBuilder<List<IxScopeBaseConfig>>
                    {
                        Config = new List<IxScopeBaseConfig>()
                    });

            return nodesBuilder;
        }

        public static async Task<IxLock<T>> Get<T>(this IIxResolver resolver, string name = null)
        {
            return new IxLock<T>(await resolver.Resolve(new IxIdentifier(typeof(T), name)));
        }
    }
}