using System;
using System.IO;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Core;

namespace Vostok.Logging.ConsoleLog.MessageWriters
{
    // (krait): measured speed: ~23k messages/sec
    // (krait): problem: can overlap with messages from ColoredWriter in a bad way
    // (krait): ~17k messages/sec if flushed after each event, which solves the problem
    internal class ColorlessMessageWriter : IMessageWriter
    {
        private readonly TextWriter writer;
        private readonly ConversionPattern pattern;

        private bool isDirty;

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
            //isDirty = true;
        }

        public void Flush()
        {
            //if (isDirty)
            //    writer.Flush();
        }
    }
}