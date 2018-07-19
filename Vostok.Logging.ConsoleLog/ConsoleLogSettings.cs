using Vostok.Logging.Core;

namespace Vostok.Logging.ConsoleLog
{
    public class ConsoleLogSettings
    {
        public ConversionPattern ConversionPattern { get; set; } = ConversionPattern.Default;

        public int EventsQueueCapacity { get; set; } = 10000;
    }
}