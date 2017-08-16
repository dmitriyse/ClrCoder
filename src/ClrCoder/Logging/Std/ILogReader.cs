// <copyright file="ILogReader.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Logging.Std
{
#if NETSTANDARD1_3 || NETSTANDARD1_6 || NETSTANDARD2_0
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Logic;
    using NodaTime;

    public interface ILogReader
    {
        ValueTask<IReadOnlyList<LogEntry>> GetLogs(Interval<Instant> interval);
    }
#endif
}