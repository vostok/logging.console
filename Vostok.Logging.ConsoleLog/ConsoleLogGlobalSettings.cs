using JetBrains.Annotations;

namespace Vostok.Logging.ConsoleLog
{
    [PublicAPI]
    public class ConsoleLogGlobalSettings
    {
        public int EventsQueueCapacity { get; set; } = 100 * 1000;

        public int OutputBufferSize { get; set; } = 65536;
    }
}