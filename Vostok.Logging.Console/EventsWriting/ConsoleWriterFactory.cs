using System.IO;

namespace Vostok.Logging.Console.EventsWriting
{
    internal class ConsoleWriterFactory : IConsoleWriterFactory
    {
        private readonly IConsoleFeaturesDetector featuresDetector;
        private readonly int bufferSize;

        public ConsoleWriterFactory(IConsoleFeaturesDetector featuresDetector, int bufferSize)
        {
            this.featuresDetector = featuresDetector;
            this.bufferSize = bufferSize;
        }

        public IConsoleWriter CreateWriter()
        {
            var colorChanger = featuresDetector.AreColorsSupported ? 
                new ConsoleColorChanger() as IConsoleColorChanger : 
                new FakeConsoleColorChanger();

            var stream = System.Console.OpenStandardOutput();
            var writer = new StreamWriter(stream, System.Console.OutputEncoding, bufferSize, true) {AutoFlush = false};
            return new ConsoleWriter(writer, colorChanger);
        }
    }
}