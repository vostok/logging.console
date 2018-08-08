using System.Threading;
using JetBrains.Annotations;
using Vostok.Logging.Abstractions;

namespace Vostok.Logging.ConsoleLog
{
    [PublicAPI]
    public class ConsoleLog : ILog
    {
        private static readonly ConsoleLogMuxerProvider MuxerProvider = new ConsoleLogMuxerProvider();

        public static void UpdateGlobalSettings([NotNull] ConsoleLogGlobalSettings newSettings) =>
            MuxerProvider.UpdateSettings(newSettings);

        public static void Flush() => MuxerProvider.ObtainMuxer().Flush();

        private readonly ConsoleLogSettings settings;
        private long eventsLost;

        public ConsoleLog([NotNull] ConsoleLogSettings settings) =>
            this.settings = SettingsValidator.ValidateInstanceSettings(settings);

        public ConsoleLog()
            : this(new ConsoleLogSettings())
        {
        }

        public long EventsLost => Interlocked.Read(ref eventsLost);

        public static long TotalEventsLost => MuxerProvider.ObtainMuxer().EventsLost;

        public void Log(LogEvent @event)
        {
            if (@event == null)
                return;

            if (!MuxerProvider.ObtainMuxer().TryLog(@event, settings))
                Interlocked.Increment(ref eventsLost);
        }

        public bool IsEnabledFor(LogLevel level) => true;

        public ILog ForContext(string context) => this;
    }
}