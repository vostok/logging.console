namespace Vostok.Logging.Console.EventsWriting
{
    internal interface IConsoleFeaturesDetector
    {
        bool AreColorsSupported { get; }
    }
}