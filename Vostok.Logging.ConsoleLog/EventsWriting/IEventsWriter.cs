namespace Vostok.Logging.ConsoleLog.EventsWriting
{
    internal interface IEventsWriter
    {
        void WriteEvents(LogEventInfo[] events, int eventsCount);
    }
}