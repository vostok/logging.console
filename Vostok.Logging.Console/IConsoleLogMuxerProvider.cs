namespace Vostok.Logging.Console
{
    internal interface IConsoleLogMuxerProvider
    {
        IConsoleLogMuxer ObtainMuxer();
    }
}