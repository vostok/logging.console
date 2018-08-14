using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Formatting;

namespace Vostok.Logging.Console
{
    /// <summary>
    /// Settings for a <see cref="ConsoleLog"/> instance.
    /// </summary>
    [PublicAPI]
    public class ConsoleLogSettings
    {
        /// <summary>
        /// The <see cref="OutputTemplate"/> used to render log messages.
        /// </summary>
        public OutputTemplate OutputTemplate { get; set; } = OutputTemplate.Default;

        /// <summary>
        /// If specified, this <see cref="IFormatProvider"/> will be used when formatting log events.
        /// </summary>
        public IFormatProvider FormatProvider { get; set; }

        /// <summary>
        /// Mapping of log levels to text color in console.
        /// </summary>
        public Dictionary<LogLevel, ConsoleColor> ColorMapping { get; set; } = new Dictionary<LogLevel, ConsoleColor>
        {
            [LogLevel.Debug] = ConsoleColor.Gray,
            [LogLevel.Info] = ConsoleColor.White,
            [LogLevel.Warn] = ConsoleColor.Yellow,
            [LogLevel.Error] = ConsoleColor.Red,
            [LogLevel.Fatal] = ConsoleColor.DarkRed,
        };

        /// <summary>
        /// Specifies whether the console log must colorize text depending on the log level.
        /// </summary>
        public bool ColorsEnabled { get; set; } = true;
    }
}