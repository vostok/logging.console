using System;
using System.Threading;
using Vostok.Logging.Console.EventsWriting;

namespace Vostok.Logging.Console
{
    internal class ConsoleLogMuxerProvider : IConsoleLogMuxerProvider
    {
        private readonly Lazy<ConsoleLogMuxer> muxer;
        private ConsoleLogGlobalSettings muxerSettings = new ConsoleLogGlobalSettings();

        public ConsoleLogMuxerProvider()
        {
            muxer = new Lazy<ConsoleLogMuxer>(
                () => CreateMuxer(muxerSettings),
                LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public void UpdateSettings(ConsoleLogGlobalSettings newSettings)
        {
            muxerSettings = SettingsValidator.ValidateGlobalSettings(newSettings);
        }

        public IConsoleLogMuxer ObtainMuxer()
        {
            return muxer.Value;
        }

        private static ConsoleLogMuxer CreateMuxer(ConsoleLogGlobalSettings settings)
        {
            var consoleFeaturesDetector = new ConsoleFeaturesDetector();
            var consoleWriterFactory = new ConsoleWriterFactory(consoleFeaturesDetector, settings.OutputBufferSize);
            var consoleWriter = consoleWriterFactory.CreateWriter();
            var eventsBatcher = new EventsBatcher();
            var eventsWriter = new EventsWriter(eventsBatcher, consoleWriter);

            return new ConsoleLogMuxer(eventsWriter, settings.EventsQueueCapacity, settings.EventsTemporaryBufferCapacity);
        }
    }
}