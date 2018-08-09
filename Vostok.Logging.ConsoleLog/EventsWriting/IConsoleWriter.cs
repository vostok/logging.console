namespace Vostok.Logging.ConsoleLog.EventsWriting
{
    internal interface IConsoleWriter
    {
        void WriteLogEvent(LogEventInfo eventInfo);

        void Flush();
    }
}