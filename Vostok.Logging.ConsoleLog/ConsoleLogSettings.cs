using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Core;

namespace Vostok.Logging.ConsoleLog
{
    [PublicAPI]
    public class ConsoleLogSettings // TODO(krait): equals
    {
        public ConversionPattern ConversionPattern { get; set; } = ConversionPattern.Default;

        public Dictionary<LogLevel, ConsoleColor> ColorMapping { get; set; } = new Dictionary<LogLevel, ConsoleColor>
        {
            {LogLevel.Debug, ConsoleColor.Gray},
            {LogLevel.Info, ConsoleColor.White},
            {LogLevel.Warn, ConsoleColor.Yellow},
            {LogLevel.Error, ConsoleColor.Red},
            {LogLevel.Fatal, ConsoleColor.Red}
        };

        public bool ColorsEnabled { get; set; } = true;

        public int StreamBufferSize { get; set; } = 4096;
    }
}