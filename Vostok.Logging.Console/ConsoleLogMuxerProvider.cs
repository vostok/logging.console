using System.Threading;
using Vostok.Logging.Console.EventsWriting;

namespace Vostok.Logging.Console
{
    internal class ConsoleLogMuxerProvider
    {
        private ConsoleLogMuxer muxer;
        private ConsoleLogGlobalSettings muxerSettings = new ConsoleLogGlobalSettings();

        public void UpdateSettings(ConsoleLogGlobalSettings newSettings) =>
            muxerSettings = SettingsValidator.ValidateGlobalSettings(newSettings);

        public ConsoleLogMuxer ObtainMuxer()
        {
            if (muxer != null)
                return muxer;

            var newMuxer = CreateMuxer(muxerSettings);
            return Interlocked.CompareExchange(ref muxer, newMuxer, null) ?? newMuxer;
        }

        private static ConsoleLogMuxer CreateMuxer(ConsoleLogGlobalSettings settings) =>
            new ConsoleLogMuxer(new EventsWriter(new EventsBatcher(), new ConsoleWriterProvider(settings.OutputBufferSize)), settings.EventsQueueCapacity);
    }
}