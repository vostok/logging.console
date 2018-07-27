using System;
using System.IO;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Core;

namespace Vostok.Logging.ConsoleLog.MessageWriters
{
    // (krait): measured speed: ~17k messages/sec
    internal class ColorlessMessageWriter : IMessageWriter
    {
        private readonly TextWriter writer;
        private readonly ConversionPattern pattern;

        public ColorlessMessageWriter(ConversionPattern pattern, int bufferSize)
        {
            this.pattern = pattern;

            var stream = Console.OpenStandardOutput(bufferSize);
            writer = new StreamWriter(stream, Console.OutputEncoding, bufferSize) {AutoFlush = false};
        }

        public void Write(LogEvent @event)
        {
            pattern.Render(@event, writer);
            writer.Flush();
        }
    }
}