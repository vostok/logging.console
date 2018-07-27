using JetBrains.Annotations;

namespace Vostok.Logging.ConsoleLog
{
    [PublicAPI]
    public class ConsoleLogGlobalSettings
    {
        public int EventsQueueCapacity { get; set; } = 10000;

        public int OutputBufferSize { get; set; } = 65536;
    }
}