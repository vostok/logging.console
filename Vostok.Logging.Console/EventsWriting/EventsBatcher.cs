using System;
using System.Collections.Generic;

namespace Vostok.Logging.Console.EventsWriting
{
    internal class EventsBatcher : IEventsBatcher
    {
        private readonly bool ignoreColors;

        internal EventsBatcher(bool ignoreColors)
        {
            this.ignoreColors = ignoreColors;
        }

        public IEnumerable<ArraySegment<LogEventInfo>> BatchEvents(LogEventInfo[] events, int eventsCount)
        {
            if (eventsCount == 0)
                yield break;

            var batchStart = 0;

            for (var i = 0; i < eventsCount; i++)
            {
                if (i == 0 || ignoreColors || FitInOneBatch(events[i - 1], events[i]))
                    continue;

                yield return new ArraySegment<LogEventInfo>(events, batchStart, i - batchStart);
                batchStart = i;
            }

            yield return new ArraySegment<LogEventInfo>(events, batchStart, eventsCount - batchStart);
        }

        private static bool FitInOneBatch(LogEventInfo a, LogEventInfo b) =>
            ReferenceEquals(a.Settings, b.Settings) &&
                (!a.Settings.ColorsEnabled || a.Event.Level == b.Event.Level) ||
            a.Settings.ColorMapping[a.Event.Level] == b.Settings.ColorMapping[b.Event.Level];
    }
}