using System;
using System.IO;
using Vostok.Logging.Abstractions;

namespace Vostok.Logging.ConsoleLog
{
    // (krait): measured speed: ~16k messages/sec with colors, ~8.5k messages/sec without colors
    internal class MessageWriter
    {
        private readonly TextWriter writer;

        public MessageWriter(int bufferSize)
        {
            var stream = Console.OpenStandardOutput(bufferSize);
            writer = new StreamWriter(stream, Console.OutputEncoding, bufferSize, true) {AutoFlush = false};
        }

        public void Write(LogEvent @event, ConsoleLogSettings settings)
        {
            if (settings.ColorsEnabled)
            {
                if (!settings.ColorMapping.TryGetValue(@event.Level, out var color))
                    color = ConsoleColor.Gray;

                using (new ConsoleColorChanger(color))
                {
                    settings.ConversionPattern.Render(@event, writer);
                    writer.Flush();
                }
            }
            else
            {
                settings.ConversionPattern.Render(@event, writer);
                writer.Flush();
            }
        }
    }
}