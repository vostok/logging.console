using Vostok.Logging.Console.Utilities;

namespace Vostok.Logging.Console.EventsWriting
{
    internal class ConsoleFeaturesDetector : IConsoleFeaturesDetector
    {
        public ConsoleFeaturesDetector()
        {
            AreColorsSupported = !(System.Console.IsOutputRedirected || OutputRedirectionDetector.IsOutputRedirected());
        }

        public bool AreColorsSupported { get; }
    }
}