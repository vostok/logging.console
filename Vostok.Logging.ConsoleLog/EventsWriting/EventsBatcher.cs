using System;
using System.Collections.Generic;

namespace Vostok.Logging.ConsoleLog
{
    internal class EventsBatcher : IEventsBatcher
    {
        public IEnumerable<ArraySegment<LogEventInfo>> BatchEvents(LogEventInfo[] events, int eventsCount)
        {
            if (eventsCount == 0)
                yield break;

            var batchStart = 0;

            for (var i = 0; i < eventsCount; i++)
            {
                if (i == 0 || FitInOneBatch(events[i - 1], events[i]))
                    continue;

                yield return new ArraySegment<LogEventInfo>(events, batchStart, i - batchStart);
                batchStart = i;
            }

            yield return new ArraySegment<LogEventInfo>(events, batchStart, eventsCount - batchStart);
        }

        private static bool FitInOneBatch(LogEventInfo a, LogEventInfo b)
        {
            if (!ReferenceEquals(a.Settings, b.Settings))
                return false;

            return !a.Settings.ColorsEnabled || a.Event.Level == b.Event.Level;
        }
    }
}