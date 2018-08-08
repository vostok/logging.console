namespace Vostok.Logging.ConsoleLog
{
    internal interface IConsoleWriter
    {
        void Flush();

        void WriteLogEvent(LogEventInfo eventInfo);
    }
}