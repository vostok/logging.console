using System;
using System.Collections.Generic;

namespace Vostok.Logging.ConsoleLog.EventsWriting
{
    internal interface IEventsBatcher
    {
        IEnumerable<ArraySegment<LogEventInfo>> BatchEvents(LogEventInfo[] events, int eventsCount);
    }
}