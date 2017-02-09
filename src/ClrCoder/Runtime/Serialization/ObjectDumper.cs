// <copyright file="ObjectDumper.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    using JetBrains.Annotations;

    /// <summary>
    /// Dumps <c>object</c> to something serializable friendly.
    /// </summary>
    /// <remarks>Filter is not thread-safe.</remarks>
    [PublicAPI]
    public class ObjectDumper : IObjectDumper
    {
        private readonly Dictionary<Type, DumpObjectFilterDelegate> _filters;

        private readonly Dictionary<object, object> _knownTargets =
            new Dictionary<object, object>(EqualityComparer<object>.Default);

        private object _prevTarget;

        private object _prevDump;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectDumper"/> class.
        /// </summary>
        /// <param name="filters">Dump filters.</param>
        public ObjectDumper(IReadOnlyDictionary<Type, DumpObjectFilterDelegate> filters)
        {
            _filters = filters.ToDictionary(x => x.Key, x => x.Value);

            if (filters == null)
            {
                throw new ArgumentNullException(nameof(filters));
            }

            if (!_filters.ContainsKey(typeof(object)))
            {
                _filters.Add(typeof(object), DefaultObjectFilter);
            }
        }

        /// <summary>
        /// Minimal filter for object type. Required for known target work.
        /// </summary>
        public static DumpObjectFilterDelegate DefaultObjectFilter { get; } = (maxType, dump, target, dumper) => dump ?? target;

        /// <inheritdoc/>
        public object Filter(Type type, object dump, object target)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (_prevTarget != target)
            {
                object result;
                if (_knownTargets.TryGetValue(target, out result))
                {
                    return result;
                }
            }

            object prevTargetCache = _prevTarget;
            object prevDumpCache = _prevDump;
            try
            {
                // Verifying that any references will be resolved after dump is created.
                if (prevTargetCache != null && target != prevTargetCache)
                {
                    if (prevDumpCache == null)
                    {
                        throw new InvalidOperationException(
                            "Object references should be dumped only after dump created.");
                    }
                }


                Type typeToFilter = type;

                DumpObjectFilterDelegate filterToCall;

                // Going up to the System.Object in the inheritance chain.
                while (!_filters.TryGetValue(typeToFilter, out filterToCall))
                {
                    if (typeToFilter == typeof(object))
                    {
                        Debug.Assert(false, "All objects are finally derived from System.Object.");
                    }

                    typeToFilter = typeToFilter.IsConstructedGenericType
                                       ? typeToFilter.GetTypeInfo().GetGenericTypeDefinition()
                                       : typeToFilter.GetTypeInfo().BaseType;
                }

                _prevTarget = target;
                _prevDump = dump;
                object resultDump = filterToCall(type, dump, target, this);

                object knownDump;
                if (_knownTargets.TryGetValue(target, out knownDump))
                {
                    if (dump!= null && !ReferenceEquals(knownDump, dump))
                    {
                        throw new InvalidOperationException("Filter should not change dump instance.");
                    }
                }
                else
                {
                    _knownTargets[target] = resultDump;
                }

                if (prevDumpCache == null)
                {
                    prevDumpCache = resultDump;
                }

                return resultDump;
            }
            finally
            {
                _prevTarget = prevTargetCache;
                _prevDump = prevDumpCache;
            }
        }
    }
}