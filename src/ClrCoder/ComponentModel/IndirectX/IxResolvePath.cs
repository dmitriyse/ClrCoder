// <copyright file="IxResolvePath.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class IxResolvePath
    {
        public IxResolvePath(IxProviderNode root, IReadOnlyList<IxProviderNode> path)
        {
            if (root == null)
            {
                throw new ArgumentNullException(nameof(root));
            }

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            // If path is empty, this means that root itself provide dependency.
            ////if (!path.Any())
            ////{
            ////    throw new ArgumentException("Path should not be empty.");
            ////}
            Root = root;
            Path = path;
        }

        public IxProviderNode Root { get; }

        public IReadOnlyList<IxProviderNode> Path { get; }

        public IxProviderNode Target => Path.Last();

        public IxResolvePath ReRoot(IxProviderNode newRoot)
        {
            return new IxResolvePath(newRoot, new[] { Root }.Concat(Path).ToArray());
        }
    }
}