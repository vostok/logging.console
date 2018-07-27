namespace Vostok.Logging.ConsoleLog
{
    // TODO(krait): Tests.
    internal static class SettingsValidator
    {
        public static void ValidateGlobalSettings(ConsoleLogGlobalSettings settings)
        {
            if (settings.EventsQueueCapacity <= 0)
                Fail($"{nameof(settings.EventsQueueCapacity)} must be positive.");
        }

        public static void ValidateInstanceSettings(ConsoleLogSettings settings)
        {
            if (settings.StreamBufferSize <= 0)
                Fail($"{nameof(settings.StreamBufferSize)} must be positive.");

            if (settings.ConversionPattern == null)
                Fail($"{nameof(settings.ConversionPattern)} must not be null.");

            if (settings.ColorMapping == null)
                Fail($"{nameof(settings.ColorMapping)} must not be null.");
        }

        private static void Fail(string message) =>
            throw new ValidationException(message);
    }
}