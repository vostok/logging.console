using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Formatting;

namespace Vostok.Logging.ConsoleLog
{
    [PublicAPI]
    public class ConsoleLogSettings
    {
        public OutputTemplate OutputTemplate { get; set; } = OutputTemplate.Default;

        public Dictionary<LogLevel, ConsoleColor> ColorMapping { get; set; } = new Dictionary<LogLevel, ConsoleColor>
        {
            {LogLevel.Debug, ConsoleColor.Gray},
            {LogLevel.Info, ConsoleColor.White},
            {LogLevel.Warn, ConsoleColor.Yellow},
            {LogLevel.Error, ConsoleColor.Red},
            {LogLevel.Fatal, ConsoleColor.DarkRed}
        };

        public bool ColorsEnabled { get; set; } = true;
    }
}