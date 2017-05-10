﻿// <copyright file="LogsController.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Logger
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  using System.Threading.Tasks;

  using Logging.Std;

  using Logic;

  using Microsoft.AspNetCore.Mvc;
  using Microsoft.CodeAnalysis.CSharp.Syntax;

  using NodaTime;
  using NodaTime.Extensions;
  using NodaTime.Text;

  [Route("api/v1/[controller]")]
  public class LogsController : Controller
  {
    private ILogReader _logReader;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogsController"/> class.
    /// </summary>
    public LogsController(ILogReader logReader)
    {
      // public LogsController()
      _logReader = logReader;
    }

    /// <summary>
    /// Gets all url entries.
    /// </summary>
    /// <param name="start"> The start instant. </param>
    /// <param name="end"> The end instant. </param>
    /// <param name="top"> The top. </param>
    /// <param name="skip"> The skip. </param>
    /// <returns>
    /// Returns all video entries.
    /// </returns>
    [HttpGet]

    public async Task<IActionResult> Get(
      [FromQuery] string start,
      [FromQuery] string end,
      [FromQuery] int? top,
      [FromQuery] int? skip)
    {
      Instant startInstant;
      Instant endInstant;
      if (start == null)
      {
        startInstant = DateTime.MinValue.ToUniversalTime().ToInstant();
      }
      else
      {
        try
        {
          startInstant = InstantPattern.ExtendedIso.Parse(start).Value;
        }
        catch
        {
          return BadRequest($"Invalid value of parameter '{nameof(start)}'");
        }
      }

      if (end == null)
      {
        endInstant = DateTime.MaxValue.ToUniversalTime().ToInstant();
      }
      else
      {
        try
        {
          endInstant = InstantPattern.ExtendedIso.Parse(end).Value;
        }
        catch (Exception ex)
        {
          return BadRequest($"Invalid value of parameter '{nameof(end)}'");
        }
      }

      IReadOnlyList<LogEntry> items = await _logReader.GetLogs(new Interval<Instant>(startInstant, endInstant));

      IQueryable<LogEntry> result = items.AsQueryable();

      if (skip != null)
      {
        result = result.Skip(skip.Value);
      }

      if (top!= null)
      {
        result = result.Take(top.Value);
      }

      return Ok(result.ToArray());
    }
  }
}