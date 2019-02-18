namespace Vostok.Logging.Console.EventsWriting
{
    internal interface IConsoleWriterFactory
    {
        IConsoleWriter CreateWriter(bool useConsoleOut = false);
    }
}