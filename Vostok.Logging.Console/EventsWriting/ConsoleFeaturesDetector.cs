namespace Vostok.Logging.Console.EventsWriting
{
    internal class ConsoleFeaturesDetector : IConsoleFeaturesDetector
    {
        public bool AreColorsSupported => !System.Console.IsOutputRedirected;
    }
}