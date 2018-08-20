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
            var featuresDetector = new ConsoleFeaturesDetector();
            return new ConsoleLogMuxer(
                new EventsWriter(new EventsBatcher(), new ConsoleWriterFactory(featuresDetector, settings.OutputBufferSize).CreateWriter()),
                settings.EventsQueueCapacity,
                settings.EventsTemporaryBufferCapacity);
        }
    }
}