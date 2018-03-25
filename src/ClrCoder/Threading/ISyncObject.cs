// <copyright file="ISyncObject.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Threading
{
    /// <summary>
    /// The synchronization object abstraction.
    /// </summary>
    public interface ISyncObject
    {
        void Enter();

        void Exit();
    }
}