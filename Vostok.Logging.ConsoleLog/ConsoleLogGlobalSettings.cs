using JetBrains.Annotations;

namespace Vostok.Logging.ConsoleLog
{
    /// <summary>
    /// Settings that are shared between all <see cref="ConsoleLog"/> instances.
    /// </summary>
    [PublicAPI]
    public class ConsoleLogGlobalSettings
    {
        /// <summary>
        /// Capacity of the log events queue.
        /// </summary>
        public int EventsQueueCapacity { get; set; } = 10 * 1000;

        /// <summary>
        /// Size of the buffer used when writing log messages to console.
        /// </summary>
        public int OutputBufferSize { get; set; } = 65536;
    }
}