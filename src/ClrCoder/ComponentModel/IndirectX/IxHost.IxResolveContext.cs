// <copyright file="IxHost.IxResolveContext.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ComponentModel.IndirectX
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    public partial class IxHost
    {
        public class IxResolveContext
        {
            [CanBeNull]
            private readonly IxResolveContext _parentContext;

            private readonly IxResolveContext _rootContext;

            private HashSet<ValueTuple<IIxInstance, IxIdentifier>> _activeResolves;

            private bool _isFailed;

            public IxResolveContext([CanBeNull] IxResolveContext parentContext)
            {
                _parentContext = parentContext;
                _rootContext = parentContext ?? this;
                if (_parentContext == null)
                {
                    _activeResolves = new HashSet<ValueTuple<IIxInstance, IxIdentifier>>();
                }
                else
                {
                    _activeResolves = _parentContext._activeResolves;
                }
            }

            public bool IsFailed => _rootContext._isFailed;

            public void RegisterResolveFinished(IIxInstance instance, IxIdentifier identifier)
            {
                if (instance == null)
                {
                    throw new ArgumentNullException(nameof(instance));
                }

                lock (_activeResolves)
                {
                    _activeResolves.Remove(new ValueTuple<IIxInstance, IxIdentifier>(instance, identifier));
                }
            }

            public void RegisterResolveStart(IIxInstance instance, IxIdentifier identifier)
            {
                if (instance == null)
                {
                    throw new ArgumentNullException(nameof(instance));
                }

                lock (_activeResolves)
                {
                    if (!_activeResolves.Add(new ValueTuple<IIxInstance, IxIdentifier>(instance, identifier)))
                    {
                        throw new InvalidOperationException("Dependency cycle found in resolve operation.");
                    }
                }
            }

            public void SetFailed()
            {
                _rootContext._isFailed = true;
            }
        }
    }
}