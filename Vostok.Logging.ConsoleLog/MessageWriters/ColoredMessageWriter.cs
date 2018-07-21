﻿using System;
using System.Collections.Generic;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Core.ConversionPattern;

namespace Vostok.Logging.ConsoleLog
{
    internal class ColoredMessageWriter : IMessageWriter
    {
        private static readonly ConversionPatternRenderer renderer = new ConversionPatternRenderer();

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
            {
                renderer.Render(pattern, @event, Console.Out);
            }
        }

        public void Flush()
        {
        }
    }
}