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
            if (newSettings == null)
                throw new ArgumentNullException(nameof(settings));

            SettingsValidator.ValidateGlobalSettings(newSettings);
            ConsoleLogMuxer.Settings = newSettings;
        }

        private readonly ConsoleLogSettings settings;

        public ConsoleLog([NotNull] ConsoleLogSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            SettingsValidator.ValidateInstanceSettings(settings);
            this.settings = settings;
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