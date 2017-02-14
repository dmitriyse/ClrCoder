// <copyright file="WebHostDeploymentConfig.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.AspNetCore
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using ComponentModel.IndirectX;

    using JetBrains.Annotations;

    /// <summary>
    /// Web host deployment configuration.
    /// </summary>
    public class WebHostDeploymentConfig : IIxStdProviderConfig
    {
        IxIdentifier IIxProviderNodeConfig.Identifier => new IxIdentifier(typeof(IWebAppComponent), Name);

        IIxVisibilityFilterConfig IIxProviderNodeConfig.ImportFilter => null;

        IIxVisibilityFilterConfig IIxProviderNodeConfig.ExportToParentFilter => null;

        IIxVisibilityFilterConfig IIxProviderNodeConfig.ExportFilter => null;

        [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1502:ElementMustNotBeOnSingleLine",
            Justification = "Reviewed. Suppression is OK here.")]
        ICollection<IIxProviderNodeConfig> IIxProviderNodeConfig.Nodes => new IIxProviderNodeConfig[] { };

        IIxScopeBindingConfig IIxStdProviderConfig.ScopeBinding => null;

        IIxMultiplicityConfig IIxStdProviderConfig.Multiplicity => null;

        IIxInstanceBuilderConfig IIxStdProviderConfig.Factory => new IxClassInstanceBuilderConfig<WebHostDeployment>();

        IxDisposeHandlerDelegate IIxStdProviderConfig.DisposeHandler => null;

        /// <summary>
        /// Registration name.
        /// </summary>
        [CanBeNull]
        public string Name { get; set; }

        /// <summary>
        /// Environment variable that can be used to <c>override</c> urls.
        /// </summary>
        [CanBeNull]
        public string UrlsEnvironmentVariableName { get; set; }
    }
}