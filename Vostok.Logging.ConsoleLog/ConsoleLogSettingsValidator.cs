using Vostok.Logging.Core;

namespace Vostok.Logging.ConsoleLog
{
    internal class ConsoleLogSettingsValidator : ILogSettingsValidator<ConsoleLogSettings>
    {
        public SettingsValidationResult TryValidate(ConsoleLogSettings settings)
        {
            if (settings?.ConversionPattern == null)
                return SettingsValidationResult.ConversionPatternIsNull();

            if (settings.EventsQueueCapacity <= 0)
                return SettingsValidationResult.CapacityIsLessThanZero();

            return SettingsValidationResult.Success();
        }
    }
}