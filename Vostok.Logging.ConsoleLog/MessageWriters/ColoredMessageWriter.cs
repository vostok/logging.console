using System;
using System.Collections.Generic;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Core;

namespace Vostok.Logging.ConsoleLog.MessageWriters
{
    // (krait): measured speed: ~4.5k messages/sec
    internal class ColoredMessageWriter : IMessageWriter
    {
        private readonly ConversionPattern pattern;
        private readonly Dictionary<LogLevel, ConsoleColor> colorMapping;

        public ColoredMessageWriter(ConversionPattern pattern, Dictionary<LogLevel, ConsoleColor> colorMapping)
        {
            this.pattern = pattern;
            this.colorMapping = colorMapping;
        }

        public void Write(LogEvent @event)
        {
            if (!colorMapping.TryGetValue(@event.Level, out var color))
                color = ConsoleColor.Gray;

            using (new ConsoleColorChanger(color))
                pattern.Render(@event, Console.Out);
        }

        public void Flush()
        {
        }
    }
}