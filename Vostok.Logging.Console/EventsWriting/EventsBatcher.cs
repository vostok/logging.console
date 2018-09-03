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

        private static bool FitInOneBatch(LogEventInfo previous, LogEventInfo current)
        {
            if (ReferenceEquals(previous.Settings, current.Settings))
                return !previous.Settings.ColorsEnabled || previous.Event.Level == current.Event.Level;

            if (!previous.Settings.ColorsEnabled && !current.Settings.ColorsEnabled)
                return true;

            if (!previous.Settings.ColorMapping.TryGetValue(previous.Event.Level, out var previousColor))
                previousColor = ConsoleColor.Gray;

            if (!current.Settings.ColorMapping.TryGetValue(current.Event.Level, out var currentColor))
                currentColor = ConsoleColor.Gray;

            return previousColor == currentColor;
        }
    }
}