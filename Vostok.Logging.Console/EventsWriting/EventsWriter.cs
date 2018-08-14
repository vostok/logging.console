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

        public EventsWriter(IEventsBatcher batcher, IConsoleWriter consoleWriter)
        {
            this.batcher = batcher;
            this.consoleWriter = consoleWriter;
        }

        public void WriteEvents(LogEventInfo[] events, int eventsCount)
        {
            foreach (var batch in batcher.BatchEvents(events, eventsCount))
                WriteBatch(batch);
        }

        private void WriteBatch(IList<LogEventInfo> batch)
        {
            var settings = batch[0].Settings;
            if (settings.ColorsEnabled)
            {
                if (!settings.ColorMapping.TryGetValue(batch[0].Event.Level, out var color))
                    color = ConsoleColor.Gray;

                using (consoleWriter.ChangeColor(color))
                {
                    WriteBatchInternal(batch);
                }
            }
            else
            {
                WriteBatchInternal(batch);
            }
        }

        private void WriteBatchInternal(IEnumerable<LogEventInfo> batch)
        {
            foreach (var eventInfo in batch)
            {
                try
                {
                    consoleWriter.WriteLogEvent(eventInfo);
                }
                catch
                {
                    // ignored
                }
            }

            consoleWriter.Flush();
        }
    }
}