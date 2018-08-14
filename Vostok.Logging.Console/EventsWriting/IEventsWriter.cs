namespace Vostok.Logging.Console.EventsWriting
{
    internal interface IEventsWriter
    {
        void WriteEvents(LogEventInfo[] events, int eventsCount);
    }
}