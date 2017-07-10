// <copyright file="AsyncResponseHandler.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.AspNetCore.AsyncResponse
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    using Logging;
    using Logging.Std;

    using Microsoft.AspNetCore.Http;

    using Threading;

    /// <summary>
    /// Handles async responses.
    /// </summary>
    /// <typeparam name="TResponseKey">The type of the response key.</typeparam>
    [PublicAPI]
    public class AsyncResponseHandler<TResponseKey>
    {
        private readonly Func<HttpContext, TResponseKey> _keyExtractor;

        private readonly bool _useRequestBodyBuffering;

        private readonly TimeSpan _handlerRegistrationTimeOut;

        private readonly Dictionary<TResponseKey, Workflow> _workflows = new Dictionary<TResponseKey, Workflow>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncResponseHandler{TResponseKey}"/> class.
        /// </summary>
        /// <param name="keyExtractor">The "key extractor" delegate.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="useRequestBodyBuffering">Enables request body buffering (required for key extraction from the body).</param>
        /// <param name="handlerRegistrationTimeOut">Maximum allowed time to register receive delegate.</param>
        public AsyncResponseHandler(
            Func<HttpContext, TResponseKey> keyExtractor,
            IJsonLogger logger,
            bool useRequestBodyBuffering = true,
            TimeSpan handlerRegistrationTimeOut = default(TimeSpan))
        {
            if (keyExtractor == null)
            {
                throw new ArgumentNullException(nameof(keyExtractor));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (handlerRegistrationTimeOut < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(handlerRegistrationTimeOut),
                    "Trap registration timeout should be positive.");
            }

            _keyExtractor = keyExtractor;
            Log = new ClassJsonLogger<AsyncResponseHandler<TResponseKey>>(logger);

            _useRequestBodyBuffering = useRequestBodyBuffering;

            if (handlerRegistrationTimeOut == default(TimeSpan))
            {
                handlerRegistrationTimeOut = TimeSpan.FromSeconds(10);
            }

            _handlerRegistrationTimeOut = handlerRegistrationTimeOut;
        }

        private IJsonLogger Log { get; set; }

        /// <summary>
        /// Handles request routed from asp.net core.
        /// </summary>
        /// <param name="httpContext">The request context..</param>
        /// <returns>Async execution TPL task.</returns>
        public async Task RequestHandler(HttpContext httpContext)
        {
            // Exceptions from this method are processed by 
            var ms = new MemoryStream();
            if (_useRequestBodyBuffering)
            {
                try
                {
                    await httpContext.Request.Body.CopyToAsync(ms);
                    ms.Position = 0;
                    httpContext.Request.Body = ms;
                }
                catch (Exception ex) when (ex.IsProcessable())
                {
                    Log.Error(_ => _("Error reading request body").Exception(ex));
                    throw;
                }
            }

            TResponseKey key;
            try
            {
                key = _keyExtractor(httpContext);
                if (_useRequestBodyBuffering)
                {
                    ((MemoryStream)httpContext.Request.Body).Position = 0;
                }
            }
            catch (Exception e)
            {
                Log.Error(_ => _("Key extraction fault from async response").Exception(e));
                throw;
            }

            Workflow workflow = GetOrCreateWorkflow(key);
            workflow.AsyncResponseReceivedPromise.SetResult(httpContext);
            await workflow.AspNetHandlingCompleted;
        }

        /// <summary>
        /// Waits async response and handle it.
        /// </summary>
        /// <param name="key">The response key.</param>
        /// <param name="responseHandler">The response handler proc.</param>
        /// <param name="timeout">Response handling timeout.</param>
        /// <returns>Async execution TPL task.</returns>
        public async Task WaitAndHandleAsyncResponse(
            TResponseKey key,
            Func<TResponseKey, HttpContext, Task> responseHandler,
            TimeSpan timeout)
        {
            if (timeout <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout should be positive.");
            }

            Workflow workflow = GetOrCreateWorkflow(key);
            workflow.HandlerRegistrationPromise.SetResult((responseHandler, timeout));
            await workflow.HandlingCompleted;
        }

        private Workflow GetOrCreateWorkflow(TResponseKey key)
        {
            Workflow workflow;
            var isWorkflowCreated = false;
            lock (_workflows)
            {
                if (!_workflows.TryGetValue(key, out workflow))
                {
                    workflow = new Workflow(this, key);
                    _workflows.Add(key, workflow);
                    isWorkflowCreated = true;
                }
            }

            if (isWorkflowCreated)
            {
                // Safe way to start task.
                workflow.Run().EnsureStarted();
            }

            return workflow;
        }

        private class Workflow
        {
            private readonly AsyncResponseHandler<TResponseKey> _owner;

            private readonly TResponseKey _key;

            private readonly TaskCompletionSource<ValueVoid> _aspNetPipelineHandlingCompletedPromise =
                new TaskCompletionSource<ValueVoid>();

            private readonly TaskCompletionSource<ValueVoid> _handlingCompletedPromise =
                new TaskCompletionSource<ValueVoid>();

            public Workflow(AsyncResponseHandler<TResponseKey> owner, TResponseKey key)
            {
                _owner = owner;
                _key = key;
            }

            public TaskCompletionSource<(Func<TResponseKey, HttpContext, Task> handler, TimeSpan responseWaitTimeout)> HandlerRegistrationPromise { get; } =
                new TaskCompletionSource<(Func<TResponseKey, HttpContext, Task> handler, TimeSpan responseWaitTimeout)
                >();

            public TaskCompletionSource<HttpContext> AsyncResponseReceivedPromise { get; } =
                new TaskCompletionSource<HttpContext>();

            public Task AspNetHandlingCompleted => _aspNetPipelineHandlingCompletedPromise.Task;

            public Task HandlingCompleted => _handlingCompletedPromise.Task;

            public async Task Run()
            {
                try
                {
                    (Func<TResponseKey, HttpContext, Task> handler, TimeSpan responseWaitTimeout) handlerParams;
                    try
                    {
                        handlerParams =
                            await HandlerRegistrationPromise.Task.TimeoutAfter(_owner._handlerRegistrationTimeOut);
                    }
                    catch (TimeoutException ex)
                    {
                        _aspNetPipelineHandlingCompletedPromise.SetException(ex);

                        _handlingCompletedPromise.SetException(
                            new InvalidOperationException(
                                "Too late to register handler, async response already processed."));

                        _owner.Log.Warning(
                            _ => _("Async response handler not registered in appropriate amount of time."));
                        return;
                    }

                    HttpContext httpContext;

                    try
                    {
                        httpContext =
                            await AsyncResponseReceivedPromise.Task.TimeoutAfter(handlerParams.responseWaitTimeout);
                    }
                    catch (TimeoutException ex)
                    {
                        _handlingCompletedPromise.SetException(ex);
                        _aspNetPipelineHandlingCompletedPromise.SetException(
                            new InvalidOperationException("Response handler already finished by a timeout."));

                        _owner.Log.Error(
                            _ => _($"Async response wait timeout (timeout={handlerParams.Item2})."));
                        return;
                    }

                    Task responseHandleTask = handlerParams.handler(_key, httpContext);
                    await Task.WhenAny(responseHandleTask);
                    if (responseHandleTask.IsFaulted)
                    {
                        _aspNetPipelineHandlingCompletedPromise.SetException(responseHandleTask.Exception);
                        _handlingCompletedPromise.SetException(responseHandleTask.Exception);
                    }
                    else
                    {
                        _aspNetPipelineHandlingCompletedPromise.SetResult(default(ValueVoid));
                        _handlingCompletedPromise.SetResult(default(ValueVoid));
                    }
                }
                finally
                {
                    lock (_owner._workflows)
                    {
                        _owner._workflows.Remove(_key);
                    }

                    // Only after this point another workflow for this key can be registered.
                }
            }
        }
    }
}