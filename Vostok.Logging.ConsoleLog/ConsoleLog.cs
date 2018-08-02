using System;
using JetBrains.Annotations;
using Vostok.Logging.Abstractions;

namespace Vostok.Logging.ConsoleLog
{
    [PublicAPI]
    public class ConsoleLog : ILog
    {
        public static void UpdateGlobalSettings([NotNull] ConsoleLogGlobalSettings newSettings)
        {
            ConsoleLogMuxer.Settings = SettingsValidator.ValidateGlobalSettings(newSettings);
        }

        private readonly ConsoleLogSettings settings;

        public ConsoleLog([NotNull] ConsoleLogSettings settings)
        {
            this.settings = SettingsValidator.ValidateInstanceSettings(settings);
        }

        public ConsoleLog()
            : this(new ConsoleLogSettings())
        {
        }

        public static int EventsLost => ConsoleLogMuxer.EventsLost;

        public void Log(LogEvent @event)
        {
            if (@event == null)
                return;

            ConsoleLogMuxer.Log(@event, settings);
        }

        public bool IsEnabledFor(LogLevel level) => true;

        public ILog ForContext(string context) => this;
    }
}