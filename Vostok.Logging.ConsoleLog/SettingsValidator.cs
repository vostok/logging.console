using System;

namespace Vostok.Logging.ConsoleLog
{
    internal static class SettingsValidator
    {
        public static ConsoleLogGlobalSettings ValidateGlobalSettings(ConsoleLogGlobalSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (settings.EventsQueueCapacity <= 0)
                Fail($"{nameof(settings.EventsQueueCapacity)} must be positive.");

            return settings;
        }

        public static ConsoleLogSettings ValidateInstanceSettings(ConsoleLogSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (settings.OutputTemplate == null)
                Fail($"{nameof(settings.OutputTemplate)} must not be null.");

            if (settings.ColorMapping == null)
                Fail($"{nameof(settings.ColorMapping)} must not be null.");

            return settings;
        }

        private static void Fail(string message) =>
            throw new ValidationException(message);
    }
}