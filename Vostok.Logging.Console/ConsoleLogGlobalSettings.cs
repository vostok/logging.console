using JetBrains.Annotations;

namespace Vostok.Logging.Console
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
        public int EventsQueueCapacity { get; set; } = 50 * 1000;

        /// <summary>
        /// Capacity of the temporary events buffer used by the background events writer.
        /// </summary>
        public int EventsTemporaryBufferCapacity { get; set; } = 10 * 1000;

        /// <summary>
        /// Size of the buffer used when writing log messages to console, in bytes.
        /// </summary>
        public int OutputBufferSize { get; set; } = 65536;
    }
}