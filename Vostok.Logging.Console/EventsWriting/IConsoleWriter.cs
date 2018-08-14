namespace Vostok.Logging.Console.EventsWriting
{
    internal interface IConsoleWriter
    {
        void WriteLogEvent(LogEventInfo eventInfo);

        void Flush();
    }
}