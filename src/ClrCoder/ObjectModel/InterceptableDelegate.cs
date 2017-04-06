// <copyright file="InterceptableDelegate.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.ObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using JetBrains.Annotations;

    /// <summary>
    /// Interceptors pattern implementation.<br/>
    /// TODO: Find link to classic pattern description.
    /// </summary>
    /// <typeparam name="TDelegate">Target delegate type.</typeparam>
    [PublicAPI]
    public class InterceptableDelegate<TDelegate>
        where TDelegate : class
    {
        private readonly TDelegate _originalDelegate;

        private readonly Dictionary<Func<TDelegate, TDelegate>, int> _interceptors =
            new Dictionary<Func<TDelegate, TDelegate>, int>();

        [CanBeNull]
        private TDelegate _delegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptableDelegate{TDelegate}"/> class.
        /// </summary>
        /// <param name="originalDelegate">Original delegate that is called after all interceptions.</param>
        public InterceptableDelegate(TDelegate originalDelegate)
        {
            if (originalDelegate == null)
            {
                throw new ArgumentNullException(nameof(originalDelegate));
            }

            _originalDelegate = originalDelegate;
        }

        /// <summary>
        /// Intercepted <c>delegate</c>.
        /// </summary>
        /// <remarks>This method is thread-safe.</remarks>
        public TDelegate Delegate
        {
            get
            {
                if (_delegate == null)
                {
                    lock (_interceptors)
                    {
                        if (_delegate == null)
                        {
                            _delegate = _interceptors.OrderBy(x => x.Value)
                                .Select(x => x.Key)
                                .BuildInterceptorsChain(_originalDelegate);
                        }
                    }
                }

                return _delegate;
            }
        }

        /// <summary>
        /// Adds interceptors.
        /// </summary>
        /// <remarks>This method is thread-safe.</remarks>
        /// <param name="interceptor">Intercept function.</param>
        /// <param name="priority">
        /// Interceptor with lowest <c>priority</c> called firstly, then it calls an <c>interceptor</c> with
        /// grater <c>priority</c>, lastly original <c>delegate</c> will be called.
        /// </param>
        public void Add(Func<TDelegate, TDelegate> interceptor, int priority)
        {
            if (interceptor == null)
            {
                throw new ArgumentNullException(nameof(interceptor));
            }

            lock (_interceptors)
            {
                _interceptors.Add(interceptor, priority);
                _delegate = null;
            }
        }
    }
}