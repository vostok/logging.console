using System;
using System.Collections.Generic;

namespace Vostok.Logging.Console.EventsWriting
{
    internal interface IEventsBatcher
    {
        IEnumerable<ArraySegment<LogEventInfo>> BatchEvents(LogEventInfo[] events, int eventsCount);
    }
}