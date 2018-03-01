// <copyright file="NUnitJsonLogger.cs" company="ClrCoder project">
// Copyright (c) ClrCoder project. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
// </copyright>

namespace ClrCoder.Testing
{
    using System;
    using System.Linq;

    using Json;

    using Logging;
    using Logging.Std;

    using NodaTime;

    using NUnit.Framework;

    using Runtime.Serialization;

    using Threading;

    /// <summary>
    /// Logger that outputs to NUnit test result.
    /// </summary>
    public class NUnitJsonLogger : IJsonLogger
    {
        private readonly DateTimeZone _localZone;

        private DedicatedThreadWorker _worker;

        /// <summary>
        /// Initializes a new instance of the <see cref="NUnitJsonLogger"/> class.
        /// </summary>
        public NUnitJsonLogger()
            : this(SyncHandler.Instance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NUnitJsonLogger"/> class.
        /// </summary>
        /// <param name="asyncHandler">Asynchronous log write handler.</param>
        /// <param name="serializerSource">The serializer source.</param>
        public NUnitJsonLogger(
            IAsyncHandler asyncHandler,
            IJsonSerializerSource serializerSource = null)
        {
            if (asyncHandler == null)
            {
                throw new ArgumentNullException(nameof(asyncHandler));
            }

            AsyncHandler = asyncHandler;

            SerializerSource = serializerSource ?? StdJsonLogging.DefaultSerializerSource;

            _localZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
        }

        /// <inheritdoc/>
        public IAsyncHandler AsyncHandler { get; }

        /// <inheritdoc/>
        public IJsonSerializerSource SerializerSource { get; }

        /// <inheritdoc/>
        public void Log(object entry)
        {
            if (!IsOutputEnabled)
            {
                return;
            }

            LogEntry logEntry = StdJsonLogging.NormalizeToLogEntry(entry, SerializerSource);

            string dotNetTypePrefix = logEntry.DotNetType == null ? string.Empty : $"{logEntry.DotNetType}: ";

            WriteLine(
                $"{logEntry.Instant.InZone(_localZone):hh:mm:ss.f}: {dotNetTypePrefix}{logEntry.Message}");

            if (logEntry.Exception != null)
            {
                var baseIntent = "  |";
                Write(baseIntent);
                WriteLine(
                    "------------------------------------- Exception ---------------------------------------");
                string intent = string.Empty;
                ExceptionDto curException = logEntry.Exception;
                do
                {
                    Write(baseIntent);
                    Write(intent);
                    if (intent != string.Empty)
                    {
                        Write("<--");
                    }

                    string name = curException.TypeFullName?.Split('.')?.Last() ?? "NullName";
                    WriteLine($"{name}: {curException.Message}");
                    curException = curException.InnerException;
                    intent += "    ";
                }
                while (curException != null);

                Write(baseIntent);
                WriteLine(
                    "---------------------------------------------------------------------------------------");
            }
        }

        /// <summary>
        /// Binds test to logger.
        /// </summary>
        /// <returns>The unbind token.</returns>
        public IDisposable BindToTest()
        {
            if (_worker != null)
            {
                throw new InvalidOperationException("Logger already bound to some test.");
            }

            _worker = new DedicatedThreadWorker();

            return new TestUnbounder(this);
        }

        private void Write(string msg)
        {
            if (_worker == null)
            {
                TestContext.Write(msg);
                TestContext.Progress.Write(msg);
            }
            else
            {
                _worker.RunAsync(
                    () =>
                        {
                            TestContext.Write(msg);
                            TestContext.Progress.Write(msg);
                        });
            }
        }

        private void WriteLine(string msg)
        {
            if (_worker == null)
            {
                TestContext.WriteLine(msg);
                TestContext.Progress.WriteLine(msg);
            }
            else
            {
                _worker.RunAsync(
                    () =>
                        {
                            TestContext.WriteLine(msg);
                            TestContext.Progress.WriteLine(msg);
                        });
            }
        }

        /// <summary>
        /// Enables or disables output to NUnit progress and log.
        /// </summary>
        public bool IsOutputEnabled { get; set; } = true;

        private class TestUnbounder : IDisposable
        {
            private readonly NUnitJsonLogger _owner;

            public TestUnbounder(NUnitJsonLogger owner)
            {
                _owner = owner;
            }

            public void Dispose()
            {
                if (_owner._worker != null)
                {
                    _owner._worker.Dispose();
                    _owner._worker = null;
                }
            }
        }
    }
}