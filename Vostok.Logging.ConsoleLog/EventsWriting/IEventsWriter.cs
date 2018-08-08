namespace Vostok.Logging.ConsoleLog
{
    internal interface IEventsWriter
    {
        void WriteEvents(LogEventInfo[] events, int eventsCount);
    }
}