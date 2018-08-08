using System.Threading;
using JetBrains.Annotations;
using Vostok.Logging.Abstractions;

namespace Vostok.Logging.ConsoleLog
{
    [PublicAPI]
    public class ConsoleLog : ILog
    {
        private static readonly ConsoleLogMuxerProvider muxerProvider = new ConsoleLogMuxerProvider();

        public static void UpdateGlobalSettings([NotNull] ConsoleLogGlobalSettings newSettings) =>
            muxerProvider.UpdateSettings(newSettings);

        private readonly ConsoleLogSettings settings;
        private long eventsLost;

        public ConsoleLog([NotNull] ConsoleLogSettings settings) =>
            this.settings = SettingsValidator.ValidateInstanceSettings(settings);

        public ConsoleLog()
            : this(new ConsoleLogSettings())
        {
        }

        public long EventsLost => Interlocked.Read(ref eventsLost);

        public static long TotalEventsLost => muxerProvider.ObtainMuxer().EventsLost;

        public void Log(LogEvent @event)
        {
            if (@event == null)
                return;

            if (!muxerProvider.ObtainMuxer().TryLog(@event, settings))
                Interlocked.Increment(ref eventsLost);
        }

        public bool IsEnabledFor(LogLevel level) => true;

        public ILog ForContext(string context) => this;
    }
}