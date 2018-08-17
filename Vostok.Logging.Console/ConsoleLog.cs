using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Vostok.Logging.Abstractions;

namespace Vostok.Logging.Console
{
    /// <summary>
    /// <para>A log which outputs messages to console.</para>
    /// <para>
    ///     The implementation is asynchronous: logged messages are not immediately rendered and written to console. 
    ///     Instead, they are added to a queue which is processed by a background worker. The capacity of the queue 
    ///     can be changed via <see cref="UpdateGlobalSettings"/>. In case of a queue overflow some events may be dropped.
    /// </para>
    /// </summary>
    [PublicAPI]
    public class ConsoleLog : ILog
    {
        private static readonly ConsoleLogMuxerProvider MuxerProvider = new ConsoleLogMuxerProvider();

        /// <summary>
        /// <para>Update settings that affect all console log instances.</para>
        /// <para>This method only works when called before the first event was logged through any console log instance.</para>
        /// </summary>
        /// <param name="newSettings"></param>
        public static void UpdateGlobalSettings([NotNull] ConsoleLogGlobalSettings newSettings) =>
            MuxerProvider.UpdateSettings(newSettings);

        /// <summary>
        /// Wait until all currently buffered log events are actually written to console.
        /// </summary>
        public static Task FlushAsync() => MuxerProvider.ObtainMuxer().FlushAsync();

        private readonly ConsoleLogSettings settings;
        private long eventsLost;

        /// <summary>
        /// <para>Create a new console log with the given settings.</para>
        /// <para>An exception will be thrown if the provided <paramref name="settings"/> are invalid.</para>
        /// </summary>
        public ConsoleLog([NotNull] ConsoleLogSettings settings) =>
            this.settings = SettingsValidator.ValidateInstanceSettings(settings);

        /// <summary>
        /// Create a new console log with default settings. Colors are enabled by default.
        /// </summary>
        public ConsoleLog()
            : this(new ConsoleLogSettings()) // TODO(krait): use cached default
        {
        }

        /// <summary>
        /// The number of events dropped by this console log instance due to events queue overflow.
        /// </summary>
        public long EventsLost => Interlocked.Read(ref eventsLost);

        /// <summary>
        /// The total number of events dropped by all console log instances due to events queue overflow.
        /// </summary>
        public static long TotalEventsLost => MuxerProvider.ObtainMuxer().EventsLost;

        /// <inheritdoc />
        public void Log(LogEvent @event)
        {
            if (@event == null)
                return;

            if (!MuxerProvider.ObtainMuxer().TryLog(@event, settings))
                Interlocked.Increment(ref eventsLost);
        }

        /// <inheritdoc />
        public bool IsEnabledFor(LogLevel level) => true;

        /// <summary>
        /// Puts the given <paramref name="context" /> string into <see cref="F:Vostok.Logging.Abstractions.WellKnownProperties.SourceContext" /> property of all events logged by this instance.
        /// </summary>
        public ILog ForContext([NotNull] string context) => 
            this.WithProperty(WellKnownProperties.SourceContext, context, true);
    }
}