// <copyright file="IxEstensions.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public static class IxEstensions
    {
        public static IIxBuilder<List<IxScopeBaseConfig>> Add<TContract>(
            this IIxBuilder<List<IxScopeBaseConfig>> nodesBuilder,
            ScopeBinding scopeBinding = null,
            string name = null,
            Type factoryType = null,
            Func<IIxResolver, Task<TContract>> factory = null,
            Action<IIxBuilder<List<IxScopeBaseConfig>>> nodes = null)
        {
            nodesBuilder.Config.Add(
                new IxComponentConfig
                    {
                        Identifier = new IxIdentifier(typeof(TContract), name),
                        Factory = factory,
                        FactoryType = factoryType,
                        ScopeBinding = scopeBinding
                    });

            nodes?.Invoke(new IxBuilder<List<IxScopeBaseConfig>>());

            return nodesBuilder;
        }

        /// <summary>
        /// Adds scope to a node.
        /// </summary>
        /// <param name="nodesBuilder">Nodes builder of a parent node.</param>
        /// <param name="name">Scope should be named.</param>
        /// <param name="exportVisibility">Scope registration export visibility rule.</param>
        /// <param name="importFromParent">Allows to import or not dependencies from parent node.</param>
        /// <param name="nodes">Action that builds children <c>nodes</c>.</param>
        /// <returns>Fluent syntax continuation.</returns>
        public static IIxBuilder<List<IxScopeBaseConfig>> AddScope(
            this IIxBuilder<List<IxScopeBaseConfig>> nodesBuilder,
            string name,
            IxExportVisibilities exportVisibility = IxExportVisibilities.DescendantScopes,
            bool importFromParent = true,
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

            var scopeConfig = new IxScopeConfig
                                  {
                                      Identifier = new IxIdentifier(typeof(IxScope), name),
                                      ExportVisibility = exportVisibility,
                                      ImportFromParent = importFromParent
                                  };

            nodesBuilder.Config.Add(scopeConfig);

            nodes?.Invoke(
                new IxBuilder<List<IxScopeBaseConfig>>
                    {
                        Config = new List<IxScopeBaseConfig>()
                    });

            return nodesBuilder;
        }

        public static Task<T> Get<T>(this IIxResolver resolver)
        {
            throw new NotImplementedException();
        }
    }
}