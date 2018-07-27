using System;
using JetBrains.Annotations;
using Vostok.Logging.Abstractions;

namespace Vostok.Logging.ConsoleLog
{
    // TODO(krait): Tests: settings validation, global settings validation
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

        public static int LostEvents => ConsoleLogMuxer.LostEvents;
        public static double AverageDrainSize => ConsoleLogMuxer.AverageDrainSize;
        public static double AverageDrainAttempts => ConsoleLogMuxer.AverageDrainAttempts;

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