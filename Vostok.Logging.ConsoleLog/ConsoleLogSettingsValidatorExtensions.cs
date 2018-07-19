using Vostok.Logging.Core;

namespace Vostok.Logging.ConsoleLog
{
    internal static class ConsoleLogSettingsValidatorExtensions
    {
        private static readonly ConsoleLogSettingsValidator validator = new ConsoleLogSettingsValidator();

        public static SettingsValidationResult Validate(this ConsoleLogSettings settings)
        {
            return validator.TryValidate(settings);
        }
    }
}