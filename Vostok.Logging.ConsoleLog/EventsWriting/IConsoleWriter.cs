namespace Vostok.Logging.ConsoleLog.EventsWriting
{
    internal interface IConsoleWriter
    {
        void Flush();

        void WriteLogEvent(LogEventInfo eventInfo);
    }
}