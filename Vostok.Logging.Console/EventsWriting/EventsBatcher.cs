using System;
using System.Collections.Generic;

namespace Vostok.Logging.Console.EventsWriting
{
    internal class EventsBatcher : IEventsBatcher
    {
        private readonly IConsoleFeaturesDetector consoleFeaturesDetector;

        public EventsBatcher(IConsoleFeaturesDetector consoleFeaturesDetector)
        {
            this.consoleFeaturesDetector = consoleFeaturesDetector;
        }

        public IEnumerable<ArraySegment<LogEventInfo>> BatchEvents(LogEventInfo[] events, int eventsCount)
        {
            if (eventsCount == 0)
                yield break;

            if (!consoleFeaturesDetector.AreColorsSupported)
            {
                yield return new ArraySegment<LogEventInfo>(events, 0, eventsCount);
                yield break;
            }

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
            var settingsAreEqual = ReferenceEquals(a.Settings, b.Settings);
            var checkEqualSettings = !a.Settings.ColorsEnabled || a.Event.Level == b.Event.Level;

            var aGetColorResult = a.Settings.ColorMapping.TryGetValue(a.Event.Level, out var aColor);
            var bGetColorResult = b.Settings.ColorMapping.TryGetValue(a.Event.Level, out var bColor);

            var checkDifferentSettings = aGetColorResult && bGetColorResult && aColor == bColor;
            var checkDifferentSettingsNotInMap = !aGetColorResult && !bGetColorResult ||
                                                 aGetColorResult && aColor == ConsoleColor.Gray ||
                                                 bGetColorResult && bColor == ConsoleColor.Gray;

            return settingsAreEqual && checkEqualSettings ||
                   checkDifferentSettings ||
                   checkDifferentSettingsNotInMap;
        }
    }
}