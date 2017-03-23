// <copyright file="HttpUriAttribute.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.AspNetCore.Validation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using JetBrains.Annotations;

    /// <summary>
    /// Validates that Url is valid and have http or https scheme.
    /// </summary>
    [PublicAPI]
    public class HttpUriAttribute : ValidationAttribute
    {
        /// <inheritdoc/>
        public override bool RequiresValidationContext => true;

        /// <inheritdoc/>
        protected override ValidationResult IsValid(
            [CanBeNull] object obj,
            [NotNull] ValidationContext validationContext)
        {
            var uri = (string)obj;
            if (string.IsNullOrWhiteSpace(uri))
            {
                return new ValidationResult(
                    "Uri cannot be null or contains only white-spaces charters.",
                    new List<string> { validationContext.MemberName });
            }

            try
            {
                var uriObj = new Uri(uri, UriKind.Absolute);
                if (uriObj.Scheme != "http" && uriObj.Scheme != "https")
                {
                    return new ValidationResult(
                        $"Only http or https schemas are allowed. Your Uri: {(string)obj}",
                        new List<string> { validationContext.MemberName });
                }

                return ValidationResult.Success;
            }
            catch (UriFormatException ex)
            {
                return new ValidationResult(
                    ex.Message + $" Your Uri: {(string)obj}",
                    new List<string> { validationContext.MemberName });
            }
        }
    }
}