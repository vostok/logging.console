﻿using System.IO;
using Vostok.Logging.Console.Utilities;

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

        public IConsoleWriter CreateWriter(bool useConsoleOut = false)
        {
            var colorChanger = featuresDetector.AreColorsSupported ? new ConsoleColorChanger() as IConsoleColorChanger : new FakeConsoleColorChanger();

            return new ConsoleWriter(ObtainTextWriter(useConsoleOut), colorChanger);
        }

        private TextWriter ObtainTextWriter(bool useConsoleOut)
        {
            if (useConsoleOut || OutputRedirectionDetector.IsOutputRedirected())
                return System.Console.Out;

            var stream = System.Console.OpenStandardOutput();
            return new StreamWriter(stream, System.Console.OutputEncoding, bufferSize, true) {AutoFlush = false};
        }
    }
}