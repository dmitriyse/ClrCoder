// <copyright file="ModelStateValidationFilter.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.AspNetCore.Mvc
{
#if NETSTANDARD1_6 || NETSTANDARD2_0
    using JetBrains.Annotations;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    /// <summary>
    /// Standard way to anser on invalid response.
    /// </summary>
    public class ModelStateValidationFilter : ActionFilterAttribute
    {
        /// <inheritdoc/>
        public override void OnActionExecuted([NotNull] ActionExecutedContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(context.ModelState);
            }
        }
    }
#endif
}