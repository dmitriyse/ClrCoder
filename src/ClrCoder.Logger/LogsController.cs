// <copyright file="LogsController.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Logger
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Logging.Std;

    using Microsoft.AspNetCore.Mvc;

    [Route("api/v1/logs")]
    public class LogsController : Controller
    {
        private ILogReader _logReader;

        public LogsController(ILogReader logReader)
        {
            _logReader = logReader;
        }

        /// <summary>
        /// Gets all url entries.
        /// </summary>
        /// <returns>Returns all video entries.</returns>
        [HttpGet]
        public async Task<IReadOnlyList<LogEntry>> Get()
        {
            return new List<LogEntry> { new LogEntry { Message = "T1" }, new LogEntry { Message = "T2" } };
        }
    }
}