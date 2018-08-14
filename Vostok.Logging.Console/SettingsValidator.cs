using System;

namespace Vostok.Logging.Console
{
    internal static class SettingsValidator
    {
        public static ConsoleLogGlobalSettings ValidateGlobalSettings(ConsoleLogGlobalSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (settings.EventsQueueCapacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(settings.EventsQueueCapacity), $"{nameof(settings.EventsQueueCapacity)} must be positive");

            return settings;
        }

        public static ConsoleLogSettings ValidateInstanceSettings(ConsoleLogSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (settings.OutputTemplate == null)
                throw new ArgumentNullException(nameof(settings.OutputTemplate));

            if (settings.ColorMapping == null)
                throw new ArgumentNullException(nameof(settings.ColorMapping));

            return settings;
        }
    }
}