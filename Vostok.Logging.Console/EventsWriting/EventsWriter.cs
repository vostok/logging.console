using System;
using System.Collections.Generic;

namespace Vostok.Logging.Console.EventsWriting
{
    // (krait): measured speed:
    // (krait): - small batches: ~16k messages/sec with colors, ~8.5k messages/sec without colors
    // (krait): - large batches: ~23k messages/sec regardless of colors
    internal class EventsWriter : IEventsWriter
    {
        private readonly IEventsBatcher batcher;
        private readonly IConsoleWriter consoleWriter;
        private readonly IConsoleFeaturesDetector consoleFeaturesDetector;

        public EventsWriter(IEventsBatcher batcher, IConsoleWriter consoleWriter, IConsoleFeaturesDetector consoleFeaturesDetector)
        {
            this.batcher = batcher;
            this.consoleWriter = consoleWriter;
            this.consoleFeaturesDetector = consoleFeaturesDetector;
        }

        public void WriteEvents(LogEventInfo[] events, int eventsCount)
        {
            foreach (var batch in batcher.BatchEvents(events, eventsCount))
                WriteBatch(batch, consoleWriter);
        }

        private void WriteBatch(IList<LogEventInfo> batch, IConsoleWriter writer)
        {
            var settings = batch[0].Settings;
            if (settings.ColorsEnabled && consoleFeaturesDetector.AreColorsSupported)
            {
                if (!settings.ColorMapping.TryGetValue(batch[0].Event.Level, out var color))
                    color = ConsoleColor.Gray;

                using (writer.ChangeColor(color))
                {
                    WriteBatchInternal(batch, writer);
                }
            }
            else
            {
                WriteBatchInternal(batch, writer);
            }
        }

        private static void WriteBatchInternal(IEnumerable<LogEventInfo> batch, IConsoleWriter writer)
        {
            foreach (var eventInfo in batch)
            {
                try
                {
                    writer.WriteLogEvent(eventInfo);
                }
                catch(Exception e)
                {
                    // ignored
                }
            }

            writer.Flush();
        }
    }
}