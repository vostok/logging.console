using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Abstractions.Wrappers;

namespace Vostok.Logging.Console
{
    /// <summary>
    /// <para>A log which outputs events to console.</para>
    /// <para>
    ///     The implementation is asynchronous: logged messages are not immediately rendered and written to console. 
    ///     Instead, they are added to a queue which is processed by a background worker. The capacity of the queue 
    ///     can be changed via <see cref="UpdateGlobalSettings"/>. In case of a queue overflow some events may be dropped.
    /// </para>
    /// </summary>
    [PublicAPI]
    public class ConsoleLog : ILog
    {
        private static readonly ConsoleLogMuxerProvider DefaultMuxerProvider = new ConsoleLogMuxerProvider();
        private static readonly ConsoleLogSettings DefaultSettings = new ConsoleLogSettings();

        /// <summary>
        /// <para>Update settings that affect all console log instances.</para>
        /// <para>This method only works when called before the first event was logged through any console log instance.</para>
        /// </summary>
        public static void UpdateGlobalSettings([NotNull] ConsoleLogGlobalSettings newSettings) =>
            DefaultMuxerProvider.UpdateSettings(newSettings);

        /// <summary>
        /// Waits until all currently buffered log events are actually written to console.
        /// </summary>
        public static Task FlushAsync() => DefaultMuxerProvider.ObtainMuxer().FlushAsync();

        /// <summary>
        /// Waits until all currently buffered log events are actually written to console.
        /// </summary>
        public static void Flush() => FlushAsync().GetAwaiter().GetResult();

        private readonly IConsoleLogMuxerProvider muxerProvider;
        private readonly ConsoleLogSettings settings;
        private long eventsLost;

        /// <summary>
        /// <para>Create a new console log with the given settings.</para>
        /// <para>An exception will be thrown if the provided <paramref name="settings"/> are invalid.</para>
        /// </summary>
        public ConsoleLog([NotNull] ConsoleLogSettings settings)
            : this(DefaultMuxerProvider, settings)
        {
        }

        /// <summary>
        /// Create a new console log with default settings. Colors are enabled by default.
        /// </summary>
        public ConsoleLog()
            : this(DefaultSettings)
        {
        }

        internal ConsoleLog(IConsoleLogMuxerProvider muxerProvider, ConsoleLogSettings settings)
        {
            this.muxerProvider = muxerProvider;
            this.settings = SettingsValidator.ValidateInstanceSettings(settings);
        }

        /// <summary>
        /// The number of events dropped by this <see cref="ConsoleLog"/> instance due to events queue overflow.
        /// </summary>
        public long EventsLost => Interlocked.Read(ref eventsLost);

        /// <summary>
        /// The total number of events dropped by all <see cref="ConsoleLog"/> instances in process due to events queue overflow.
        /// </summary>
        public static long TotalEventsLost => DefaultMuxerProvider.ObtainMuxer().EventsLost;

        /// <inheritdoc />
        public void Log(LogEvent @event)
        {
            if (@event == null)
                return;

            if (!muxerProvider.ObtainMuxer().TryLog(@event, settings))
                Interlocked.Increment(ref eventsLost);
        }

        /// <inheritdoc />
        public bool IsEnabledFor(LogLevel level) => true;

        /// <summary>
        /// Returns a log based on this <see cref="ConsoleLog"/> instance that puts given <paramref name="context" /> string into <see cref="F:Vostok.Logging.Abstractions.WellKnownProperties.SourceContext" /> property of all logged events.
        /// </summary>
        public ILog ForContext(string context) =>
            context == null ? (ILog)this : new SourceContextWrapper(this, context);
    }
}