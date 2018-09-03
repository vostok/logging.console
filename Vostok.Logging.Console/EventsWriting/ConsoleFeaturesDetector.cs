namespace Vostok.Logging.Console.EventsWriting
{
    internal class ConsoleFeaturesDetector : IConsoleFeaturesDetector
    {
        public ConsoleFeaturesDetector()
        {
            AreColorsSupported = !System.Console.IsOutputRedirected;
        }

        public bool AreColorsSupported { get; }
    }
}