using System;
using System.IO;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Core.ConversionPattern;

namespace Vostok.Logging.ConsoleLog
{
    internal class ColorlessMessageWriter : IMessageWriter
    {
        private static readonly ConversionPatternRenderer renderer = new ConversionPatternRenderer();

        private readonly TextWriter writer;
        private readonly ConversionPattern pattern;

        private bool isDirty;

        public ColorlessMessageWriter(ConversionPattern pattern, int bufferSize)
        {
            this.pattern = pattern;

            var stream = Console.OpenStandardOutput(bufferSize);
            writer = new StreamWriter(stream);
        }

        public void Write(LogEvent @event)
        {
            renderer.Render(pattern, @event, writer);

            isDirty = true;
        }

        public void Flush()
        {
            if (isDirty)
                writer.Flush();
        }
    }
}