// <copyright file="ComponentDisposedException.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder
{
    /// <summary>
    /// A sort of OperationCanceledException that is raised when component disposes during operation.
    /// </summary>
    public class ComponentDisposedException : ComponentFaultException
    {
        public ComponentDisposedException()
            : base("Component disposed.")
        {
        }

        public ComponentDisposedException(string message)
            : base(message)
        {
        }
    }
}