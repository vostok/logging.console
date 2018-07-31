using System;
using System.IO;
using Vostok.Logging.Core;

namespace Vostok.Logging.ConsoleLog
{
    // (krait): measured speed:
    // (krait): - small batches: ~16k messages/sec with colors, ~8.5k messages/sec without colors
    // (krait): - large batches: ~23k messages/sec regardless of colors
    internal class EventsWriter
    {
        private readonly TextWriter writer;

        public EventsWriter(int bufferSize)
        {
            var stream = Console.OpenStandardOutput(bufferSize);
            writer = new StreamWriter(stream, Console.OutputEncoding, bufferSize, true) {AutoFlush = false};
        }

        public void WriteEvents(LogEventInfo[] events, int eventsCount)
        {
            var batchStart = 0;

            for (var i = 0; i < eventsCount; i++)
            {
                if (i == 0 || FitInOneBatch(events[i - 1], events[i]))
                    continue;

                WriteBatch(events, batchStart, i);
                batchStart = i;
            }

            WriteBatch(events, batchStart, eventsCount);
        }

        private static bool FitInOneBatch(LogEventInfo a, LogEventInfo b)
        {
            if (!ReferenceEquals(a.Settings, b.Settings))
                return false;

            return !a.Settings.ColorsEnabled || a.Event.Level == b.Event.Level;
        }

        private void WriteBatch(LogEventInfo[] events, int batchStart, int batchEnd)
        {
            try
            {
                var settings = events[batchStart].Settings;
                if (settings.ColorsEnabled)
                {
                    if (!settings.ColorMapping.TryGetValue(events[batchStart].Event.Level, out var color))
                        color = ConsoleColor.Gray;

                    using (new ConsoleColorChanger(color))
                    {
                        WriteInternal(events, batchStart, batchEnd, settings.ConversionPattern);
                    }
                }
                else
                {
                    WriteInternal(events, batchStart, batchEnd, settings.ConversionPattern);
                }
            }
            catch
            {
                // ignored
            }
        }

        private void WriteInternal(LogEventInfo[] events, int batchStart, int batchEnd, ConversionPattern pattern)
        {
            for (var i = batchStart; i < batchEnd; i++)
                pattern.Render(events[i].Event, writer);
            writer.Flush();
        }
    }
}