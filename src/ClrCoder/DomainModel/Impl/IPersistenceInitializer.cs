// <copyright file="IPersistenceInitializer.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.DomainModel.Impl
{
    /// <summary>
    /// Persistence initialization helper abstraction. Pass collection of <c>this</c> contracts to the constructor of your
    /// persistence.
    /// </summary>
    /// <typeparam name="TPersistence">Type of persistence to build.</typeparam>
    public interface IPersistenceInitializer<in TPersistence>
        where TPersistence : IPersistenceImpl
    {
        /// <summary>
        /// Initializes <c>persistence</c> inside constructor.
        /// </summary>
        /// <param name="persistence">Persistence to configure.</param>
        void Initialize(TPersistence persistence);
    }
}