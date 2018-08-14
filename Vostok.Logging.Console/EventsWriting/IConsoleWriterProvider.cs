namespace Vostok.Logging.Console.EventsWriting
{
    internal interface IConsoleWriterProvider
    {
        IConsoleWriter ObtainWriter();
    }
}